import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { FiCheck, FiEdit2, FiTrash2 } from "react-icons/fi";
import { useProfile } from "@/hooks/useProfile";
import { deleteTask, loadProjectTasks, loadMyTeam } from "client/api";
import { CompleteTaskModal } from "./CompleteTaskModal";
import type { ProjectTaskType } from "client/shared";

interface ProjectTasksProps {
  projectId: string;
  subjectId: string;
  isTeacher?: boolean;
  onTaskEdit?: (task: ProjectTaskType) => void;
}

export const ProjectTasks = ({
  projectId,
  subjectId,
  isTeacher,
  onTaskEdit,
}: ProjectTasksProps) => {
  const queryClient = useQueryClient();
  const { data: profile } = useProfile();
  const isUserTeacher = profile?.isTeacher || isTeacher;

  const {
    data: tasks,
    isLoading,
    isError,
  } = useQuery(loadProjectTasks(projectId));
  const { data: myTeam } = useQuery(loadMyTeam(subjectId));

  const [selectedTaskId, setSelectedTaskId] = useState<string | null>(null);

  const deleteTaskMutation = useMutation({
    mutationFn: (taskId: string) => deleteTask(taskId),
    onSuccess: () => {
      queryClient.refetchQueries({
        queryKey: loadProjectTasks(projectId).queryKey,
      });
    },
  });

  if (isLoading)
    return <p className="p-4 pl-12 text-gray-400">Загрузка задач...</p>;
  if (isError)
    return <p className="p-4 pl-12 text-red-500">Ошибка загрузки задач</p>;
  if (!tasks?.length)
    return (
      <p className="p-4 pl-12 text-gray-400">В этом проекте пока нет задач</p>
    );

  return (
    <div className="bg-gray-50">
      {tasks.map((task, index) => {
        const teamProgressForMe = myTeam
          ? task.teamProgress.find((tp) => tp.teamId === myTeam.id)
          : null;

        let taskBgClass = "";
        let taskStatusText = "";
        let taskStatusClass = "";

        if (teamProgressForMe) {
          if (teamProgressForMe.status === 2) {
            taskBgClass = "bg-green-50";
            taskStatusText = "Зачтено";
            taskStatusClass = "text-green-700";
          } else if (teamProgressForMe.status === 1) {
            taskBgClass = "bg-orange-50";
            taskStatusText = "В процессе";
            taskStatusClass = "text-orange-700";
          }
        }

        return (
          <div
            key={task.id ?? index}
            className={`
              bg-white flex justify-between items-center px-12 py-3 text-sm
              ${taskBgClass} ${index !== tasks.length - 1 ? "mb-2" : ""}
              hover:bg-gray-50
            `}
          >
            <div>
              <div className="font-medium">{task.name}</div>
              <div className="text-gray-500 text-xs">{task.description}</div>
            </div>

            <div className="flex items-center gap-2">
              {teamProgressForMe && (
                <span
                  className={`px-2 py-0.5 text-xs rounded ${taskStatusClass}`}
                >
                  {taskStatusText}
                </span>
              )}

              {isUserTeacher && (
                <>
                  <button
                    onClick={() => setSelectedTaskId(task.id)}
                    className="flex items-center gap-1 px-3 py-1.5 text-xs
                             bg-green-50 text-green-700 border border-green-200
                             rounded-md hover:bg-green-100 transition"
                  >
                    <FiCheck />
                    Засчитать
                  </button>

                  <button
                    onClick={() => onTaskEdit?.(task)}
                    className="p-2 rounded-md border border-gray-200 text-gray-400 hover:text-blue-600 hover:bg-blue-50 hover:border-blue-200 transition"
                    aria-label="Редактировать задачу"
                  >
                    <FiEdit2 className="w-4 h-4" />
                  </button>

                  <button
                    onClick={() => deleteTaskMutation.mutate(task.id)}
                    className="p-2 rounded-md border border-gray-200 text-gray-400 hover:text-red-600 hover:bg-red-50 hover:border-red-200 transition"
                    aria-label="Удалить задачу"
                  >
                    <FiTrash2 className="w-4 h-4" />
                  </button>
                </>
              )}
            </div>
          </div>
        );
      })}

      {selectedTaskId && isUserTeacher && (
        <CompleteTaskModal
          taskId={selectedTaskId}
          subjectId={subjectId}
          onClose={() => setSelectedTaskId(null)}
        />
      )}
    </div>
  );
};
