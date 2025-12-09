import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiCall } from "client/utils";

interface CreateStageModalProps {
  taskId: string;
  onClose: () => void;
  stage?: {
    id: string;
    name: string;
    description: string;
    estimatedEffort: number;
  };
}

export const CreateStageModal = ({
  taskId,
  onClose,
  stage,
}: CreateStageModalProps) => {
  const queryClient = useQueryClient();
  const isEdit = !!stage;

  const [name, setName] = useState(stage?.name ?? "");
  const [description, setDescription] = useState(stage?.description ?? "");
  const [estimatedEffort, setEstimatedEffort] = useState(
    stage?.estimatedEffort ?? 0
  );

  const mutation = useMutation({
    mutationFn: () =>
      isEdit && stage
        ? apiCall.put(
            "/stages/edit",
            { name, description, estimatedEffort },
            { params: { stageId: stage.id } }
          )
        : apiCall.post(
            "/tasks/stages/create",
            { name, description, estimatedEffort },
            { params: { taskId } }
          ),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["task-stages", taskId],
      });
      onClose();
      setName("");
      setDescription("");
      setEstimatedEffort(0);
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
          {isEdit ? "Редактировать этап" : "Добавить этап"}
        </h2>

        <div className="space-y-4">
          <input
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Название этапа"
            className="w-full border rounded-lg p-3"
          />

          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Описание этапа"
            className="w-full border rounded-lg p-3 min-h-[80px]"
          />

          <div>
            <label className="block text-sm text-gray-700 mb-1">
              Оценка усилий (часы)
            </label>
            <input
              type="number"
              value={estimatedEffort}
              onChange={(e) => setEstimatedEffort(Number(e.target.value))}
              className="w-full border rounded-lg p-3"
              min={0}
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
            disabled={mutation.isPending || !name.trim()}
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
              : "Добавить этап"}
          </button>
        </div>
      </div>
    </div>
  );
};
