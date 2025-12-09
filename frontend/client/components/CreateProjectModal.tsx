import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createProject } from "@/api";
import { loadSubjectProjects } from "client/api";

interface Props {
  subjectId: string;
  onClose: () => void;
}

export const CreateProjectModal = ({ subjectId, onClose }: Props) => {
  const queryClient = useQueryClient();

  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");

  const mutation = useMutation({
    mutationFn: () =>
      createProject(subjectId, {
        name,
        description,
        startDate,
        endDate,
      }),

    onSuccess: async () => {
      await queryClient.refetchQueries({
        queryKey: loadSubjectProjects(subjectId).queryKey,
      });

      onClose();
      setName("");
      setDescription("");
      setStartDate("");
      setEndDate("");
    },
  });

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div
        className="absolute inset-0 bg-black/30 backdrop-blur-sm"
        onClick={onClose}
      />

      <div
        className="relative z-10 bg-white w-full max-w-lg rounded-xl shadow-xl p-6 animate-fade-in"
        onClick={(e) => e.stopPropagation()}
      >
        <h2 className="text-2xl font-semibold mb-5">
          Создание итогового проекта
        </h2>

        <div className="space-y-4">
          <input
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Название проекта"
            className="w-full border rounded-lg p-3"
          />

          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Описание проекта"
            className="w-full border rounded-lg p-3 min-h-[90px]"
          />

          <div className="grid grid-cols-2 gap-3">
            <input
              type="datetime-local"
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
              className="w-full border rounded-lg p-3"
            />

            <input
              type="datetime-local"
              value={endDate}
              onChange={(e) => setEndDate(e.target.value)}
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
            disabled={mutation.isPending || !name || !startDate || !endDate}
            className={`px-5 py-2 rounded-lg text-white transition
              ${
                mutation.isPending
                  ? "bg-gray-400"
                  : "bg-blue-600 hover:bg-blue-700"
              }
            `}
          >
            {mutation.isPending ? "Создание..." : "Создать проект"}
          </button>
        </div>
      </div>
    </div>
  );
};
