import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { apiCall } from "client/utils";

interface TeamProgress {
  teamId: string;
  teamName: string;
  status: number;
}

interface TaskInfo {
  id: string;
  name: string;
  description: string;
  teamProgress: TeamProgress[];
}

interface Props {
  taskId: string;
  subjectId: string;
  onClose: () => void;
}

export const CompleteTaskModal = ({ taskId, onClose }: Props) => {
  const queryClient = useQueryClient();
  const { data: taskInfo, isLoading } = useQuery<TaskInfo>({
    queryKey: ["task-info", taskId],
    queryFn: () => apiCall.get<TaskInfo>("/tasks/info", { params: { taskId } }),
  });

  const [selectedTeams, setSelectedTeams] = useState<string[]>([]);

  if (taskInfo && selectedTeams.length === 0) {
    const alreadyCompleted = taskInfo.teamProgress
      .filter((tp) => tp.status === 2)
      .map((tp) => tp.teamId);
    setSelectedTeams(alreadyCompleted);
  }

  const toggleTeam = (teamId: string) => {
    setSelectedTeams((prev) =>
      prev.includes(teamId)
        ? prev.filter((id) => id !== teamId)
        : [...prev, teamId]
    );
  };

  const completeMutation = useMutation({
    mutationFn: () =>
      apiCall.post("/tasks/complete-for-teams", {
        teamIds: selectedTeams,
        taskIds: [taskId],
      }),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["project-tasks"],
      });
      onClose();
    },
  });

  if (isLoading)
    return (
      <div className="p-6 text-gray-500">Загрузка информации о задаче...</div>
    );

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-5">
      <div
        className="absolute inset-0 bg-black/30 backdrop-blur-sm"
        onClick={onClose}
      />

      <div
        className="relative bg-white rounded-xl shadow-xl p-6 w-full max-w-lg"
        onClick={(e) => e.stopPropagation()}
      >
        <h2 className="text-2xl font-semibold mb-4">{taskInfo?.name}</h2>
        <p className="text-gray-600 mb-4">{taskInfo?.description}</p>

        <div className="max-h-80 overflow-y-auto space-y-2 mb-5">
          {taskInfo?.teamProgress.map((team) => {
            const isCompleted = team.status === 2;
            return (
              <label
                key={team.teamId}
                className={`flex items-center justify-between p-3
                           bg-gray-50 rounded-lg cursor-pointer
                           hover:bg-gray-100 transition ${
                             isCompleted ? "opacity-70 cursor-not-allowed" : ""
                           }`}
              >
                <span className="font-medium">{team.teamName}</span>
                <input
                  type="checkbox"
                  checked={selectedTeams.includes(team.teamId)}
                  disabled={isCompleted}
                  onChange={() => toggleTeam(team.teamId)}
                  className="w-5 h-5"
                />
              </label>
            );
          })}
        </div>

        <div className="flex justify-end gap-2">
          <button
            onClick={onClose}
            className="px-4 py-2 border rounded-lg hover:bg-gray-100"
          >
            Отмена
          </button>

          <button
            onClick={() => completeMutation.mutate()}
            disabled={!selectedTeams.length || completeMutation.isPending}
            className={`px-4 py-2 rounded-lg text-white transition ${
              !selectedTeams.length || completeMutation.isPending
                ? "bg-gray-400 cursor-not-allowed"
                : "bg-green-600 hover:bg-green-700"
            }`}
          >
            {completeMutation.isPending ? "Сохранение..." : "Сохранить"}
          </button>
        </div>
      </div>
    </div>
  );
};
