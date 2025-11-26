import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createTeam } from "client/api";
import { useState } from "react";

interface CreateTeamModalProps {
  subjectId: string;
  setIsModalOpen: (isOpen: boolean) => void;
}

export const CreateTeamModal = ({
  subjectId,
  setIsModalOpen,
}: CreateTeamModalProps) => {
  const [teamName, setTeamName] = useState("");
  const [teamDescription, setTeamDescription] = useState("");

  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: () =>
      createTeam(subjectId, {
        name: teamName,
        description: teamDescription,
        techStack: "",
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["teams", subjectId],
      });
      setIsModalOpen(false);
      setTeamName("");
      setTeamDescription("");
    },
    onError: (err) => {
      console.error("Ошибка при создании команды:", err);
    },
  });

  const handleCreateTeam = () => {
    if (!teamName.trim()) return;
    mutation.mutate();
  };

  return (
    <div className="fixed inset-0 flex items-center justify-center z-50">
      <div
        className="absolute inset-0 bg-black/20 backdrop-blur-sm"
        onClick={() => setIsModalOpen(false)}
      />

      <div
        className="bg-white rounded-xl shadow-xl w-full max-w-md p-6 relative z-10"
        onClick={(e) => e.stopPropagation()}
      >
        <h2 className="text-xl font-semibold mb-4">Создать команду</h2>

        <label className="block mb-2 text-gray-700">Имя команды</label>
        <input
          type="text"
          value={teamName}
          onChange={(e) => setTeamName(e.target.value)}
          className="w-full border border-gray-300 rounded-lg p-2 mb-4"
        />

        <label className="block mb-2 text-gray-700">Описание</label>
        <textarea
          value={teamDescription}
          onChange={(e) => setTeamDescription(e.target.value)}
          className="w-full border border-gray-300 rounded-lg p-2 mb-4"
        />

        <div className="flex justify-end gap-2">
          <button
            onClick={() => setIsModalOpen(false)}
            className="px-4 py-2 rounded-lg border border-gray-300 hover:bg-gray-100 transition"
          >
            Отмена
          </button>
          <button
            onClick={handleCreateTeam}
            disabled={mutation.isPending || !teamName.trim()}
            className={`px-4 py-2 rounded-lg text-white transition ${
              mutation.isPending || !teamName.trim()
                ? "bg-gray-400 cursor-not-allowed"
                : "bg-green-500 hover:bg-green-600"
            }`}
          >
            {mutation.isPending ? "Создание..." : "Создать"}
          </button>
        </div>
      </div>
    </div>
  );
};
