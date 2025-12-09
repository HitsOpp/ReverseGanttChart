import { teamsKeyFactory } from "@/api";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiCall } from "client/utils";

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
  const queryClient = useQueryClient();

  const leaveMutation = useMutation({
    mutationFn: () => apiCall.post(`/Teams/${team.id}/leave`),
    onSuccess: () => {
      queryClient.setQueryData(
        teamsKeyFactory.loadMyTeam(subjectId),
        undefined
      );

      queryClient.invalidateQueries({ queryKey: [subjectId, "teams"] });
      console.log(
        queryClient.getQueryData(teamsKeyFactory.loadMyTeam(subjectId))
      );
      if (onLeave) onLeave();
    },
  });

  const isLoading = leaveMutation.status === "pending";

  return (
    <div className="bg-white shadow-lg rounded-xl p-6 max-w-2xl">
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-3xl font-bold">{team.name}</h2>
        <button
          onClick={() => leaveMutation.mutate()}
          disabled={isLoading}
          className={`px-4 py-2 rounded-lg font-semibold transition ${
            isLoading
              ? "bg-gray-400 cursor-not-allowed text-white"
              : "bg-red-500 hover:bg-red-600 text-white"
          }`}
        >
          {isLoading ? "Выход..." : "Выйти"}
        </button>
      </div>

      <p className="text-gray-700 mb-6">{team.description}</p>

      <h3 className="text-xl font-semibold mb-2">
        Участники ({team.memberCount}):
      </h3>
      <ul className="space-y-1">
        {team.members.map((m) => (
          <li
            key={m.userId}
            className="flex items-center justify-between p-2 border-b last:border-b-0 rounded"
          >
            <span>{m.fullName}</span>
            <span className="text-sm text-gray-500">{m.techStack || "—"}</span>
          </li>
        ))}
      </ul>
    </div>
  );
};
