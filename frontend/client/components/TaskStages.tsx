import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { FiEdit2, FiTrash2, FiCheck, FiX } from "react-icons/fi";
import { useProfile } from "@/hooks/useProfile";
import { apiCall } from "client/utils";
import { CreateStageModal } from "./CreateStageModal";
import { CompleteStageModal } from "./CompleteStageModal";
import { UncompleteStageModal } from "./UncompleteStageModal";
import { loadMyTeam } from "@/api";

interface StageType {
  id: string;
  name: string;
  description: string;
  estimatedEffort: number;
  teamProgress: {
    teamId: string;
    teamName: string;
    isCompleted: boolean;
    completedAt?: string;
    completedByName?: string;
  }[];
}

interface TaskStagesProps {
  taskId: string;
  subjectId: string;
  isTeacher?: boolean;
}

export const TaskStages = ({
  taskId,
  isTeacher,
  subjectId,
}: TaskStagesProps) => {
  const queryClient = useQueryClient();
  const { data: profile } = useProfile();
  const { data: myTeam } = useQuery(loadMyTeam(subjectId));
  const canCheckStages = profile?.isTeacher || isTeacher;
  const canManageStages = profile?.isTeacher;

  const {
    data: stages,
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["task-stages", taskId],
    queryFn: async () => {
      const res = await apiCall.get("/tasks/stages", { params: { taskId } });
      return res as StageType[];
    },
  });

  const [editingStage, setEditingStage] = useState<StageType | null>(null);
  const [completeStageData, setCompleteStageData] = useState<{
    stageId: string;
    taskTeams: StageType["teamProgress"];
  } | null>(null);
  const [uncompleteStageData, setUncompleteStageData] = useState<{
    stageId: string;
    taskTeams: StageType["teamProgress"];
  } | null>(null);

  const deleteStageMutation = useMutation({
    mutationFn: (stageId: string) =>
      apiCall.delete("/stages/delete", { params: { stageId } }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["task-stages", taskId] });
    },
  });

  if (isLoading)
    return <div className="ml-20 p-4 text-gray-400">Загрузка этапов...</div>;

  if (isError)
    return <div className="ml-20 p-4 text-red-500">Ошибка загрузки этапов</div>;

  if (!stages?.length)
    return (
      <div className="ml-0 sm:ml-13 bg-white border border-gray-100 rounded-xl">
        <p className="p-4 text-gray-400">В этой задаче пока нет этапов</p>

        {canManageStages && (
          <div className="px-4 pb-4">
            <button
              onClick={() =>
                setEditingStage({
                  id: "",
                  name: "",
                  description: "",
                  estimatedEffort: 0,
                  teamProgress: [],
                })
              }
              className="px-3 py-2 border border-gray-200 rounded-md text-sm text-gray-600 hover:bg-gray-50"
            >
              + Добавить этап
            </button>
          </div>
        )}

        {editingStage && (
          <CreateStageModal
            taskId={taskId}
            stage={undefined}
            onClose={() => setEditingStage(null)}
          />
        )}
      </div>
    );

  return (
    <div className="ml-0 sm:ml-13 bg-white border border-gray-100 rounded-xl overflow-hidden">
      {stages.map((stage) => {
        const myProgress = stage.teamProgress?.[0];

        const isCompleted = myProgress?.isCompleted && myTeam;

        const bgClass = isCompleted ? "bg-green-50" : "";
        const statusText = isCompleted ? "Зачтено" : "";
        const statusClass = "text-green-700";

        return (
          <div
            key={stage.id}
            className={`
              flex flex-col sm:flex-row sm:justify-between sm:items-center gap-3 px-4 sm:px-6 py-3 text-sm
              border-b border-gray-100 last:border-none
              transition ${bgClass}
            `}
          >
            <div className="flex-1">
              <div className="font-medium text-gray-800">{stage.name}</div>

              <div className="text-gray-500 text-xs mt-0.5">
                {stage.description}
              </div>

              {isCompleted && (
                <span
                  className={`inline-block mt-1 px-2 py-0.5 text-xs rounded ${statusClass}`}
                >
                  {statusText}
                </span>
              )}
            </div>

            {(canCheckStages || canManageStages) && (
              <div className="flex items-center gap-2">
                {canCheckStages && (
                  <>
                    <button
                      onClick={() =>
                        setCompleteStageData({
                          stageId: stage.id,
                          taskTeams: stage.teamProgress,
                        })
                      }
                      className="px-3 py-1.5 text-xs bg-green-50 text-green-700 border border-green-200 rounded-md hover:bg-green-100"
                    >
                      <FiCheck />
                    </button>

                    <button
                      onClick={() =>
                        setUncompleteStageData({
                          stageId: stage.id,
                          taskTeams: stage.teamProgress,
                        })
                      }
                      className="px-3 py-1.5 text-xs bg-red-50 text-red-700 border border-red-200 rounded-md hover:bg-red-100"
                    >
                      <FiX />
                    </button>
                  </>
                )}

                {canManageStages && (
                  <>
                    <button
                      onClick={() => setEditingStage(stage)}
                      className="p-2 rounded-md border border-gray-200 text-gray-400 hover:text-blue-600 hover:bg-blue-50"
                    >
                      <FiEdit2 className="w-4 h-4" />
                    </button>

                      <button
                        onClick={() => deleteStageMutation.mutate(stage.id)}
                        className="p-2 rounded-md border border-gray-200 text-gray-400 hover:text-red-600 hover:bg-red-50"
                      >
                        <FiTrash2 className="w-4 h-4" />
                      </button>
                  </>
                )}
              </div>
            )}
          </div>
        );
      })}

      {canManageStages && (
        <div className="px-6 py-3 bg-gray-50 border-t border-gray-100">
          <button
            onClick={() =>
              setEditingStage({
                id: "",
                name: "",
                description: "",
                estimatedEffort: 0,
                teamProgress: [],
              })
            }
            className="px-3 py-2 border border-gray-200 rounded-md text-sm text-gray-600 hover:bg-gray-100"
          >
            + Добавить этап
          </button>
        </div>
      )}

      {editingStage && (
        <CreateStageModal
          taskId={taskId}
          stage={editingStage.id ? editingStage : undefined}
          onClose={() => setEditingStage(null)}
        />
      )}

      {completeStageData && (
        <CompleteStageModal
          stageId={completeStageData.stageId}
          taskTeams={completeStageData.taskTeams}
          onClose={() => setCompleteStageData(null)}
        />
      )}

      {uncompleteStageData && (
        <UncompleteStageModal
          stageId={uncompleteStageData.stageId}
          taskTeams={uncompleteStageData.taskTeams}
          onClose={() => setUncompleteStageData(null)}
        />
      )}
    </div>
  );
};
