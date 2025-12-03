import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { createTeam, teamsKeyFactory } from "@/api";

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
    onSuccess: (createdTeam) => {
      queryClient.setQueryData(
        teamsKeyFactory.loadMyTeam(subjectId),
        createdTeam
      );
      queryClient.invalidateQueries({
        queryKey: teamsKeyFactory.loadAllTeams(subjectId),
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
    <div className="fixed inset-0 flex p-6 items-center justify-center z-50">
      {/* Полностью затемнённый фон с blur */}
      <div
        className="absolute inset-0  bg-black/20 backdrop-blur-sm"
        onClick={() => setIsModalOpen(false)}
      />

      <div
        className="bg-white rounded-xl shadow-xl w-full max-w-md p-6 relative z-10"
        onClick={(e) => e.stopPropagation()}
      >
        <h2 className="text-2xl font-semibold mb-4">Создать команду</h2>

        <input
          type="text"
          value={teamName}
          onChange={(e) => setTeamName(e.target.value)}
          placeholder="Название команды"
          className="w-full border border-gray-300 rounded-lg p-3 mb-4"
        />

        <textarea
          value={teamDescription}
          onChange={(e) => setTeamDescription(e.target.value)}
          placeholder="Описание"
          className="w-full border border-gray-300 rounded-lg p-4 mb-4"
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
