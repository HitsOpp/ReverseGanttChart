import { useEffect, useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiCall } from "client/utils";

interface TeamProgress {
  teamId: string;
  teamName: string;
  isCompleted?: boolean;
}

interface UncompleteStageModalProps {
  stageId: string;
  taskTeams: TeamProgress[];
  onClose: () => void;
}

export const UncompleteStageModal = ({
  stageId,
  taskTeams,
  onClose,
}: UncompleteStageModalProps) => {
  const queryClient = useQueryClient();
  const [selectedTeams, setSelectedTeams] = useState<string[]>([]);

  useEffect(() => {
    const completedTeams = taskTeams
      .filter((t) => t.isCompleted)
      .map((t) => t.teamId);
    setSelectedTeams(completedTeams);
  }, [taskTeams]);

  const toggleTeam = (id: string, disabled: boolean) => {
    if (disabled) return;

    setSelectedTeams((prev) =>
      prev.includes(id) ? prev.filter((t) => t !== id) : [...prev, id]
    );
  };

  const mutation = useMutation({
    mutationFn: async () => {
      await apiCall.post("/stages/uncomplete-for-teams", {
        teamIds: selectedTeams,
        stageIds: [stageId],
      });
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["task-stages"] });
      onClose();
    },
  });

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div className="absolute inset-0 bg-black/30" onClick={onClose} />

      <div
        className="relative bg-white w-full max-w-md rounded-xl shadow-xl p-6"
        onClick={(e) => e.stopPropagation()}
      >
        <h2 className="text-xl font-semibold mb-4">Снять зачёт по этапу</h2>

        <div className="space-y-3 max-h-[240px] overflow-y-auto mb-6">
          {taskTeams.map((team) => {
            const isCompleted = !!team.isCompleted;
            const isChecked = selectedTeams.includes(team.teamId);

            return (
              <label
                key={team.teamId}
                className={`flex items-center gap-3 p-2 border rounded-lg ${
                  isCompleted
                    ? "cursor-pointer hover:bg-red-50"
                    : "bg-gray-50 text-gray-400 cursor-not-allowed"
                }`}
              >
                <input
                  type="checkbox"
                  checked={isChecked}
                  disabled={!isCompleted}
                  onChange={() => toggleTeam(team.teamId, !isCompleted)}
                  className="w-4 h-4"
                />
                <span className="text-sm">{team.teamName}</span>
              </label>
            );
          })}
        </div>

        <div className="flex justify-end gap-3">
          <button
            onClick={onClose}
            className="px-4 py-2 rounded-lg border hover:bg-gray-100"
          >
            Отмена
          </button>

          <button
            disabled={!selectedTeams.length || mutation.isPending}
            onClick={() => mutation.mutate()}
            className="px-4 py-2 rounded-lg bg-red-600 text-white hover:bg-red-700 disabled:opacity-50"
          >
            {mutation.isPending ? "Сохранение..." : "Снять зачёт"}
          </button>
        </div>
      </div>
    </div>
  );
}

