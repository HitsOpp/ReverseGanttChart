import { useEffect, useState } from "react";
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

export const UncompleteTaskModal = ({ taskId, onClose }: Props) => {
  const queryClient = useQueryClient();

  const { data: taskInfo, isLoading } = useQuery<TaskInfo>({
    queryKey: ["task-info", taskId],
    queryFn: () => apiCall.get<TaskInfo>("/tasks/info", { params: { taskId } }),
    enabled: !!taskId,
  });

  const [selectedTeams, setSelectedTeams] = useState<string[]>([]);

  useEffect(() => {
    if (taskInfo) {
      const completed = taskInfo.teamProgress
        .filter((tp) => tp.status === 2)
        .map((tp) => tp.teamId);

      setSelectedTeams(completed);
    }
  }, [taskInfo]);

  const toggleTeam = (teamId: string, disabled: boolean) => {
    if (disabled) return;

    setSelectedTeams((prev) =>
      prev.includes(teamId)
        ? prev.filter((id) => id !== teamId)
        : [...prev, teamId]
    );
  };

  const uncompleteMutation = useMutation({
    mutationFn: () =>
      apiCall.post("/tasks/uncomplete-for-teams", {
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
      <div className="fixed inset-0 z-50 flex items-center justify-center">
        <div className="bg-white p-6 rounded-xl shadow text-gray-500">
          Загрузка задачи...
        </div>
      </div>
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
        <h2 className="text-2xl font-semibold mb-1">
          Снять зачёт по задаче
        </h2>
        <p className="text-gray-500 mb-5">{taskInfo?.description}</p>

        <div className="max-h-80 overflow-y-auto space-y-2 mb-6">
          {taskInfo?.teamProgress.map((team) => {
            const isCompleted = team.status === 2;
            const isChecked = selectedTeams.includes(team.teamId);

            return (
              <button
                key={team.teamId}
                onClick={() => toggleTeam(team.teamId, !isCompleted)}
                className={`w-full flex items-center justify-between p-3 rounded-lg border text-sm transition
                  ${
                    !isCompleted
                      ? "bg-gray-50 border-gray-200 text-gray-400 cursor-not-allowed"
                      : isChecked
                      ? "bg-red-50 border-red-300 text-red-700"
                      : "bg-white hover:bg-red-50 hover:border-red-300"
                  }
                `}
              >
                <span className="font-medium">{team.teamName}</span>

                <input
                  type="checkbox"
                  checked={isChecked}
                  disabled={!isCompleted}
                  readOnly
                  className="w-5 h-5"
                />
              </button>
            );
          })}
        </div>

        <div className="flex justify-end gap-3">
          <button
            onClick={onClose}
            className="px-4 py-2 border rounded-lg hover:bg-gray-100"
          >
            Отмена
          </button>

          <button
            onClick={() => uncompleteMutation.mutate()}
            disabled={!selectedTeams.length || uncompleteMutation.isPending}
            className={`px-4 py-2 rounded-lg text-white transition ${
              !selectedTeams.length || uncompleteMutation.isPending
                ? "bg-gray-400 cursor-not-allowed"
                : "bg-red-600 hover:bg-red-700"
            }`}
          >
            {uncompleteMutation.isPending ? "Сохранение..." : "Снять зачёт"}
          </button>
        </div>
      </div>
    </div>
  );
}

