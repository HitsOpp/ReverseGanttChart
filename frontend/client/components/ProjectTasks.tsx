import { loadProjectTasks } from "client/api";
import { useQuery } from "@tanstack/react-query";

interface ProjectTasksProps {
  projectId: string;
}

export const ProjectTasks = ({ projectId }: ProjectTasksProps) => {
  const { data, isLoading, isError } = useQuery(loadProjectTasks(projectId));

  if (isLoading)
    return <p className="p-4 pl-12 text-gray-400">Загрузка задач...</p>;

  if (isError)
    return <p className="p-4 pl-12 text-red-500">Ошибка загрузки задач</p>;

  if (!data?.length)
    return (
      <p className="p-4 pl-12 text-gray-400">В этом проекте пока нет задач</p>
    );

  return (
    <div className="bg-gray-50">
      {data.map((task, index) => (
        <div
          key={task.id ?? index}
          className={`
            flex justify-between items-center px-12 py-3 text-sm
            ${index !== data.length - 1 ? "border-b border-gray-200" : ""}
          `}
        >
          <div>
            <div className="font-medium">{task.name}</div>
            <div className="text-gray-500 text-xs">{task.description}</div>
          </div>
        </div>
      ))}
    </div>
  );
};
