import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createSubject, subjectKeyFactory } from "client/api";

interface CreateSubjectModalProps {
  setIsModalOpen: (isOpen: boolean) => void;
}

export const CreateSubjectModal = ({
  setIsModalOpen,
}: CreateSubjectModalProps) => {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [color, setColor] = useState("#8B5CF6");
  const [errorMessage, setErrorMessage] = useState("");

  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: () => createSubject({ name, description, color }),
    onSuccess: () => {
      setErrorMessage("");
      queryClient.invalidateQueries({
        queryKey: subjectKeyFactory.loadSubject(),
      });
      setIsModalOpen(false);
      setName("");
      setDescription("");
    },
    onError: (error) => {
      console.error("Ошибка при создании предмета:", error);
      setErrorMessage("Не удалось создать предмет. Проверьте данные или авторизацию.");
    },
  });

  const handleCreate = () => {
    if (!name.trim()) return;
    mutation.mutate();
  };

  return (
    <div className="fixed inset-0 flex p-6 items-center justify-center z-50">
      <div
        className="absolute inset-0 bg-black/20 backdrop-blur-sm"
        onClick={() => setIsModalOpen(false)}
      />

      <div
        className="bg-white rounded-xl shadow-xl w-full max-w-md p-6 relative z-10"
        onClick={(e) => e.stopPropagation()}
      >
        <h2 className="text-2xl font-semibold mb-4">Создать предмет</h2>

        <input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="Название предмета"
          className="w-full border border-gray-300 rounded-lg p-3 mb-4"
        />

        <textarea
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          placeholder="Описание"
          className="w-full border border-gray-300 rounded-lg p-4 mb-4"
        />

        <div className="flex items-center gap-3 mb-4">
          <label className="text-sm text-gray-600">Цвет карточки</label>
          <input
            type="color"
            value={color}
            onChange={(e) => setColor(e.target.value)}
            className="w-10 h-10 p-0 border border-gray-300 rounded"
          />
        </div>

        {errorMessage && (
          <p className="text-sm text-red-500 mb-4">{errorMessage}</p>
        )}

        <div className="flex justify-end gap-2">
          <button
            onClick={() => setIsModalOpen(false)}
            className="px-4 py-2 rounded-lg border border-gray-300 hover:bg-gray-100 transition"
          >
            Отмена
          </button>
          <button
            onClick={handleCreate}
            disabled={mutation.isPending || !name.trim()}
            className={`px-4 py-2 rounded-lg text-white transition ${
              mutation.isPending || !name.trim()
                ? "bg-gray-400 cursor-not-allowed"
                : "bg-blue-600 hover:bg-blue-700"
            }`}
          >
            {mutation.isPending ? "Создание..." : "Создать"}
          </button>
        </div>
      </div>
    </div>
  );
};
