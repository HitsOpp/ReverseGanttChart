import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { FiPlus, FiUserPlus } from "react-icons/fi";
import { CreateTeamModal } from "./CreateTeamModal";
import { MyTeamView } from "./MyTeamView";
import { loadMyTeam, loadAllTeams, teamsKeyFactory } from "@/api/teams";
import { apiCall } from "client/utils";
import type { LoadAllTeams } from "@/shared";

interface TeamTabProps {
  subjectId: string;
}

export const TeamTab = ({ subjectId }: TeamTabProps) => {
  const queryClient = useQueryClient();
  const [isModalOpen, setIsModalOpen] = useState(false);

  const { data: myTeam } = useQuery(loadMyTeam(subjectId));

  const { data: teams, isLoading } = useQuery({
    ...loadAllTeams(subjectId),
    enabled: !myTeam,
  });

  const joinMutation = useMutation({
    mutationFn: (teamId: string) =>
      apiCall.post(
        "/Teams/join",
        { techStack: "back" },
        { params: { teamId } }
      ),

    onMutate: async (teamId) => {
      await queryClient.cancelQueries({
        queryKey: teamsKeyFactory.loadAllTeams(subjectId),
      });

      const teams = queryClient.getQueryData<LoadAllTeams[]>(
        teamsKeyFactory.loadAllTeams(subjectId)
      );

      const joinedTeam = teams?.find((t) => t.id === teamId);

      if (joinedTeam) {
        queryClient.setQueryData(
          teamsKeyFactory.loadMyTeam(subjectId),
          joinedTeam
        );
      }

      return { teams };
    },

    onError: (_err, _id, context) => {
      queryClient.setQueryData(
        teamsKeyFactory.loadAllTeams(subjectId),
        context?.teams
      );
    },

    onSettled: () => {
      queryClient.invalidateQueries({
        queryKey: teamsKeyFactory.loadAllTeams(subjectId),
      });
    },
  });

  if (myTeam) {
    return <MyTeamView subjectId={subjectId} team={myTeam} />;
  }

  return (
    <>
      <div className="p-6 bg-white shadow-sm rounded-lg overflow-hidden flex justify-between mb-4 border-b border-gray-200">
        <h1 className="text-4xl font-normal">Команды</h1>

        <button
          onClick={() => setIsModalOpen(true)}
          className="px-3 py-2 bg-green-50 text-green-700 border border-green-200 rounded-lg hover:bg-green-100 transition flex items-center gap-2"
        >
          <FiPlus /> Создать команду
        </button>
      </div>

      <div className="bg-white shadow-sm rounded-lg overflow-hidden p-2">
        {isLoading && (
          <p className="p-4 text-gray-400 text-sm">Загрузка команд...</p>
        )}

        {!isLoading && !teams?.length && (
          <p className="p-3 text-gray-500">В данном предмете ещё нет команд</p>
        )}

        {teams?.map((team) => (
          <div
            key={team.id}
            className="p-4 flex justify-between items-center border-b border-gray-200 last:border-none"
          >
            <div>
              <div className="text-lg font-medium">{team.name}</div>
              <div className="text-gray-500 text-sm">
                Участников: {team.memberCount}
              </div>
            </div>

            <button
              onClick={() => joinMutation.mutate(team.id)}
              disabled={joinMutation.status === "pending"}
              className={`px-3 py-2 border rounded-lg flex gap-2 items-center transition ${
                joinMutation.status === "pending"
                  ? "bg-gray-400 cursor-not-allowed text-white"
                  : "bg-blue-50 text-blue-700 border-blue-200 hover:bg-blue-100"
              }`}
            >
              <FiUserPlus />
              {joinMutation.status === "pending"
                ? "Присоединение..."
                : "Присоединиться"}
            </button>
          </div>
        ))}

        {isModalOpen && (
          <CreateTeamModal
            setIsModalOpen={setIsModalOpen}
            subjectId={subjectId}
          />
        )}
      </div>
    </>
  );
};
