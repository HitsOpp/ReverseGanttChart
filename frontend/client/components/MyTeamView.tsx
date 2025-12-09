import { teamsKeyFactory } from "@/api";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiCall } from "client/utils";
import { FiUser } from "react-icons/fi";

interface Member {
  userId: string;
  fullName: string;
  email: string;
  techStack: string;
  joinedAt: string;
}

interface Team {
  id: string;
  name: string;
  description: string;
  memberCount: number;
  members: Member[];
}

interface MyTeamViewProps {
  team: Team;
  subjectId: string;
  onLeave?: () => void;
}

export const MyTeamView = ({ team, subjectId, onLeave }: MyTeamViewProps) => {
  const reloadComponent = () => {
    window.location.reload();
  };
  const queryClient = useQueryClient();
  const teamId = team.id;
  const leaveMutation = useMutation({
    mutationFn: () =>
      apiCall.post(
        "/Teams/leave",
        {},
        {
          params: { teamId },
        }
      ),

    onSuccess: async () => {
      queryClient.removeQueries({
        queryKey: teamsKeyFactory.loadMyTeam(subjectId),
      });

      await queryClient.refetchQueries({
        queryKey: teamsKeyFactory.loadAllTeams(subjectId),
      });

      await queryClient.refetchQueries({
        queryKey: teamsKeyFactory.loadMyTeam(subjectId),
      });
      reloadComponent();
      if (onLeave) onLeave();
    },
  });

  const isLoading = leaveMutation.status === "pending";

  return (
    <div className="bg-white shadow-lg rounded-xl p-6 max-w-2xl">
      <div className="flex justify-between items-center mb-5">
        <h2 className="text-3xl font-bold text-gray-900">{team.name}</h2>
        <button
          onClick={() => leaveMutation.mutate()}
          disabled={isLoading}
          className={`px-5 py-2 rounded-lg font-semibold transition ${
            isLoading
              ? "bg-gray-400 cursor-not-allowed text-white"
              : "bg-red-500 hover:bg-red-600 text-white shadow"
          }`}
        >
          {isLoading ? "Выход..." : "Выйти"}
        </button>
      </div>

      <p className="text-gray-700 mb-6">{team.description}</p>

      <h3 className="text-xl font-semibold mb-4 text-gray-800">
        Участники ({team.memberCount})
      </h3>
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
        {team.members.map((m) => (
          <div
            key={m.userId}
            className="flex items-center justify-between p-3 bg-gray-50 rounded-lg shadow-sm hover:shadow-md transition"
          >
            <div className="flex items-center gap-3">
              <FiUser className="w-6 h-6 text-blue-500" />
              <div className="font-medium text-gray-800">{m.fullName}</div>
            </div>
            <span className="text-sm text-gray-500">{m.techStack || "—"}</span>
          </div>
        ))}
      </div>
    </div>
  );
};
