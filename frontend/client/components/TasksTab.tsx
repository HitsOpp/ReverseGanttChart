import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  FiPlusSquare,
  FiClipboard,
  FiChevronDown,
  FiEdit2,
  FiTrash2,
} from "react-icons/fi";
import { useProfile } from "@/hooks/useProfile";
import { deleteProject, loadSubjectProjects } from "client/api";
import type { loadProjectType, ProjectTaskType } from "client/shared";
import { useState } from "react";
import {
  ProjectTasks,
  CreateProjectModal,
  CreateTaskModal,
} from "client/components";

interface TasksTabProps {
  subjectId: string;
}

export const TasksTab = ({ subjectId }: TasksTabProps) => {
  const queryClient = useQueryClient();
  const [isProjectModalOpen, setIsProjectModalOpen] = useState(false);
  const [editingProject, setEditingProject] = useState<loadProjectType | null>(
    null
  );
  const [taskModalProjectId, setTaskModalProjectId] = useState<string | null>(
    null
  );
  const [editingTask, setEditingTask] = useState<ProjectTaskType | null>(null);
  const { data: Profile } = useProfile();
  const isTeacher = Profile?.isTeacher && Profile.isTeacher;

  const { data, isLoading, isError } = useQuery(loadSubjectProjects(subjectId));

  const [openedProjectId, setOpenedProjectId] = useState<string | null>(null);

  const deleteProjectMutation = useMutation({
    mutationFn: (projectId: string) => deleteProject(projectId),
    onSuccess: () => {
      queryClient.refetchQueries({
        queryKey: loadSubjectProjects(subjectId).queryKey,
      });
    },
  });

  return (
    <>
      <div className="bg-white shadow-sm rounded-lg overflow-hidden p-6 flex justify-between items-center">
        <h1 className="text-4xl font-normal">Итоговые проекты</h1>

        {isTeacher && (
          <button
            onClick={() => {
              setEditingProject(null);
              setIsProjectModalOpen(true);
            }}
            className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
          >
            <FiPlusSquare className="w-5 h-5" />
            Добавить проект
          </button>
        )}
      </div>

      <div className="mt-5 bg-white shadow-sm rounded-lg overflow-hidden">
        {isLoading && <p className="p-4 text-gray-500">Загрузка...</p>}

        {isError && (
          <p className="p-4 text-red-500">Ошибка при загрузке проектов</p>
        )}

        {!isLoading && !data?.length && (
          <p className="p-4 text-gray-500">Итоговых проектов пока нет</p>
        )}

        {data?.map((project, index) => {
          const isOpen = openedProjectId === project.id;

          return (
            <div
              key={project.id ?? index}
              className={`
                ${index !== data.length - 1 ? "border-b border-gray-200" : ""}
              `}
            >
              <div
                onClick={() => setOpenedProjectId(isOpen ? null : project.id)}
                className="p-4 flex justify-between items-center cursor-pointer hover:bg-gray-50"
              >
                <div className="flex items-start gap-3">
                  <FiChevronDown
                    className={`
                      w-5 h-5 mt-1 transition-transform
                      ${isOpen ? "rotate-180" : ""}
                    `}
                  />

                  <FiClipboard className="w-5 h-5 text-gray-500 mt-1" />

                  <div>
                    <div className="font-medium text-lg">{project.name}</div>

                    <div className="text-gray-500 text-sm">
                      {project.description}
                    </div>

                    <div className="text-gray-400 text-xs mt-1">
                      {new Date(project.startDate).toLocaleDateString()} —{" "}
                      {new Date(project.endDate).toLocaleDateString()}
                    </div>
                  </div>
                </div>

                {isTeacher && (
                  <div
                    className="flex items-center gap-2"
                    onClick={(e) => e.stopPropagation()}
                  >
                    <button
                      onClick={() => {
                        setEditingProject(project);
                        setIsProjectModalOpen(true);
                      }}
                      className="p-2 rounded-md border border-gray-200 text-gray-400 hover:text-blue-600 hover:bg-blue-50 hover:border-blue-200 transition"
                      aria-label="Редактировать проект"
                    >
                      <FiEdit2 className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => deleteProjectMutation.mutate(project.id)}
                      className="p-2 rounded-md border border-gray-200 text-gray-400 hover:text-red-600 hover:bg-red-50 hover:border-red-200 transition"
                      aria-label="Удалить проект"
                    >
                      <FiTrash2 className="w-4 h-4" />
                    </button>
                  </div>
                )}
              </div>

              {isOpen && (
                <ProjectTasks projectId={project.id} subjectId={subjectId} />
              )}
              {isOpen && <ProjectTasks projectId={project.id} />}
                <>
                  <ProjectTasks
                    projectId={project.id}
                    isTeacher={isTeacher}
                    onTaskEdit={(task) => {
                      setTaskModalProjectId(project.id);
                      setEditingTask(task);
                    }}
                  />

                  {isTeacher && (
                    <div className="px-12 py-3 flex justify-start border-t border-gray-100 bg-white">
                      <button
                        onClick={() => {
                          setTaskModalProjectId(project.id);
                          setEditingTask(null);
                        }}
                        className="inline-flex items-center px-3 py-2 rounded-md border border-gray-200 text-gray-600 text-sm hover:bg-gray-50 hover:border-blue-200 hover:text-blue-700 transition"
                        aria-label="Добавить задачу"
                      >
                        <span className="leading-none">+ Добавить задачу</span>
                      </button>
                    </div>
                  )}
                </>
              )}
            </div>
          );
        })}
      </div>

      {isProjectModalOpen && (
        <CreateProjectModal
          subjectId={subjectId}
          project={editingProject ?? undefined}
          onClose={() => setIsProjectModalOpen(false)}
        />
      )}

      {taskModalProjectId && (
        <CreateTaskModal
          projectId={taskModalProjectId}
          task={editingTask ?? undefined}
          onClose={() => {
            setTaskModalProjectId(null);
            setEditingTask(null);
          }}
        />
      )}
    </>
  );
};
