import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createTask, editTask } from "@/api";
import { loadProjectTasks } from "client/api";
import type { ProjectTaskType } from "client/shared";

interface CreateTaskModalProps {
  projectId: string;
  onClose: () => void;
  task?: ProjectTaskType;
}

export const CreateTaskModal = ({
  projectId,
  onClose,
  task,
}: CreateTaskModalProps) => {
  const queryClient = useQueryClient();

  const isEdit = !!task;

  const [name, setName] = useState(task?.name ?? "");
  const [description, setDescription] = useState(task?.description ?? "");
  const [dueDate, setDueDate] = useState(
    task?.dueDate ? task.dueDate.slice(0, 16) : ""
  );

  const mutation = useMutation({
    mutationFn: () =>
      isEdit && task
        ? editTask(task.id, {
            name,
            description,
            dueDate: dueDate ? new Date(dueDate).toISOString() : undefined,
          })
        : createTask(projectId, {
            name,
            description,
            dueDate: dueDate ? new Date(dueDate).toISOString() : undefined,
          }),
    onSuccess: async () => {
      await queryClient.refetchQueries({
        queryKey: loadProjectTasks(projectId).queryKey,
      });

      onClose();
      setName("");
      setDescription("");
    },
  });

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div
        className="absolute inset-0 bg-black/30 backdrop-blur-sm"
        onClick={onClose}
      />

      <div
        className="relative z-10 bg-white w-full max-w-md rounded-xl shadow-xl p-6"
        onClick={(e) => e.stopPropagation()}
      >
        <h2 className="text-2xl font-semibold mb-4">
          {isEdit ? "Редактировать задачу" : "Добавить задачу"}
        </h2>

        <div className="space-y-4">
          <input
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Название задачи"
            className="w-full border rounded-lg p-3"
          />

          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Описание задачи"
            className="w-full border rounded-lg p-3 min-h-[80px]"
          />

          <div>
            <label className="block text-sm text-gray-700 mb-1">
              Дедлайн задачи
            </label>
            <input
              type="datetime-local"
              value={dueDate}
              onChange={(e) => setDueDate(e.target.value)}
              className="w-full border rounded-lg p-3"
            />
          </div>
        </div>

        <div className="flex justify-end gap-3 mt-6">
          <button
            onClick={onClose}
            className="px-5 py-2 rounded-lg border hover:bg-gray-100"
          >
            Отмена
          </button>

          <button
            onClick={() => mutation.mutate()}
            disabled={mutation.isPending || !name.trim() || !dueDate}
            className={`px-5 py-2 rounded-lg text-white transition ${
              mutation.isPending
                ? "bg-gray-400"
                : "bg-blue-600 hover:bg-blue-700"
            }`}
          >
            {mutation.isPending
              ? isEdit
                ? "Сохранение..."
                : "Добавление..."
              : isEdit
                ? "Сохранить"
                : "Добавить задачу"}
          </button>
        </div>
      </div>
    </div>
  );
};
