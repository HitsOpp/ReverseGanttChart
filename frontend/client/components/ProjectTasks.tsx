import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  FiCheck,
  FiEdit2,
  FiTrash2,
  FiChevronDown,
  FiClock,
  FiX,
} from "react-icons/fi";
import { useProfile } from "@/hooks/useProfile";
import {
  deleteTask,
  loadProjectTasks,
  loadMyTeam,
  loadSubjectRole,
} from "client/api";
import { CompleteTaskModal } from "./CompleteTaskModal";
import { UncompleteTaskModal } from "./UncompleteTaskModal";
import { TaskStages } from "./TaskStages";
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
  const { data: role } = useQuery(loadSubjectRole(subjectId));
  const { data: profile } = useProfile();
  const isUserTeacher = profile?.isTeacher || isTeacher;
  const isAssistant = role?.role === "Assist";
  const canCheckTasks = isUserTeacher || isAssistant;

  const {
    data: tasks,
    isLoading,
    isError,
  } = useQuery(loadProjectTasks(projectId));
  const { data: myTeam } = useQuery(loadMyTeam(subjectId));

  const [selectedTaskId, setSelectedTaskId] = useState<string | null>(null);
  const [uncompleteTaskId, setUncompleteTaskId] = useState<string | null>(null);
  const [openedTaskId, setOpenedTaskId] = useState<string | null>(null);

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

  const now = new Date();

  return (
    <div className="bg-gray-50">
      {tasks.map((task, index) => {
        const teamProgressForMe = myTeam
          ? task.teamProgress.find((tp) => tp.teamId === myTeam.id)
          : null;

        let taskBgClass = "";
        let taskStatusText = "";
        let taskStatusClass = "";
        let isOverdue = false;

        if (teamProgressForMe) {
          if (teamProgressForMe.status === 2) {
            taskBgClass = "bg-green-50";
            taskStatusText = "Зачтено";
            taskStatusClass = "text-green-700";
          } else if (teamProgressForMe.completedStageCount >= 1) {
            taskBgClass = "bg-orange-50";
            taskStatusText = "В процессе";
            taskStatusClass = "text-orange-700";
          }

          const dueDate = task.dueDate ? new Date(task.dueDate) : null;
          if (dueDate && dueDate < now && teamProgressForMe.status !== 2) {
            isOverdue = true;
            taskBgClass = "bg-red-50";
            taskStatusText = "Просрочено";
            taskStatusClass = "text-red-700";
          }
        }

        const isOpen = openedTaskId === task.id;
        const dueDate = task.dueDate ? new Date(task.dueDate) : null;
        const formattedDueDate = dueDate
          ? dueDate.toLocaleString("ru-RU", {
              day: "2-digit",
              month: "2-digit",
              year: "numeric",
              hour: "2-digit",
              minute: "2-digit",
            })
          : null;

        return (
          <div key={task.id ?? index} className="mb-2">
            <div
              onClick={() => setOpenedTaskId(isOpen ? null : task.id)}
              className={`
                flex justify-between items-center px-12 py-3 text-sm
                cursor-pointer transition
                ${taskBgClass}
                hover:bg-gray-100
              `}
            >
              <div className="flex items-center gap-3">
                <FiChevronDown
                  className={`transition-transform ${
                    isOpen ? "rotate-180" : ""
                  }`}
                />
                <div>
                  <div className="font-medium">{task.name}</div>
                  <div className="text-gray-500 text-xs">
                    {task.description}
                  </div>

                  {formattedDueDate && (
                    <div className="flex items-center gap-1 mt-1 text-xs text-gray-500">
                      <FiClock />
                      {formattedDueDate}
                    </div>
                  )}
                </div>
              </div>

              <div className="flex items-center gap-2">
                {teamProgressForMe && (
                  <span
                    className={`px-2 py-0.5 text-xs rounded ${taskStatusClass}`}
                  >
                    {taskStatusText}
                  </span>
                )}

                {canCheckTasks && (
                  <>
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        setSelectedTaskId(task.id);
                      }}
                      className="px-3 py-1.5 text-xs bg-green-50 text-green-700
                                 border border-green-200 rounded-md hover:bg-green-100"
                    >
                      <FiCheck />
                    </button>

                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        setUncompleteTaskId(task.id);
                      }}
                      className="px-3 py-1.5 text-xs bg-red-50 text-red-700 border border-red-200 rounded-md hover:bg-red-100"
                    >
                      <FiX />
                    </button>
                  </>
                )}

                {isUserTeacher && (
                  <>
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        onTaskEdit?.(task);
                      }}
                      className="p-2 border rounded-md text-gray-400 hover:text-blue-600"
                    >
                      <FiEdit2 className="w-4 h-4" />
                    </button>

                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        deleteTaskMutation.mutate(task.id);
                      }}
                      className="p-2 border rounded-md text-gray-400 hover:text-red-600"
                    >
                      <FiTrash2 className="w-4 h-4" />
                    </button>
                  </>
                )}
              </div>
            </div>

            {isOpen && (
              <TaskStages
                taskId={task.id}
                subjectId={subjectId}
                isTeacher={canCheckTasks}
              />
            )}
          </div>
        );
      })}

      {selectedTaskId && canCheckTasks && (
        <CompleteTaskModal
          taskId={selectedTaskId}
          subjectId={subjectId}
          onClose={() => setSelectedTaskId(null)}
        />
      )}

      {uncompleteTaskId && canCheckTasks && (
        <UncompleteTaskModal
          taskId={uncompleteTaskId}
          subjectId={subjectId}
          onClose={() => setUncompleteTaskId(null)}
        />
      )}
    </div>
  );
};
