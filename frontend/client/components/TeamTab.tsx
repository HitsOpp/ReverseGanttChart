import { useState } from "react";
import { loadAllTeams } from "@/api";
import { useQuery } from "@tanstack/react-query";
import { FiPlus, FiUserPlus } from "react-icons/fi";
import { CreateTeamModal } from "./CreateTeamModal";

interface TeamTabProps {
  subjectId: string;
}

export const TeamTab = ({ subjectId }: TeamTabProps) => {
  const { data } = useQuery(loadAllTeams(subjectId));
  const [isModalOpen, setIsModalOpen] = useState(false);

  return (
    <>
      <div className="p-6 bg-white shadow-sm rounded-lg overflow-hidden flex justify-between mb-4 border-b border-gray-200">
        <h1 className="text-4xl font-normal">Команды</h1>
        <button
          onClick={() => setIsModalOpen(true)}
          className="px-3 py-2 bg-green-50 text-green-700 border border-green-200 rounded-lg hover:bg-green-100 transition flex items-center gap-2"
        >
          <FiPlus />
          Создать команду
        </button>
      </div>
      <div className="bg-white shadow-sm rounded-lg overflow-hidden p-2">
        {data?.map((team, index) => (
          <div
            key={team.id}
            className={`
            p-4 flex justify-between items-center
            ${index !== data.length - 1 ? "border-b border-gray-200" : ""}
          `}
          >
            <div>
              <div className="text-lg font-medium">{team.name}</div>
              <div className="text-gray-500 text-sm">
                Участников: {team.memberCount}
              </div>
            </div>

            <div className="flex gap-2">
              <button className="px-3 py-2 bg-blue-50 text-blue-700 border border-blue-200 rounded-lg flex gap-2 items-center hover:bg-blue-100 transition">
                <FiUserPlus />
                Присоединиться
              </button>
            </div>
          </div>
        ))}
        {isModalOpen && (
          <CreateTeamModal
            setIsModalOpen={setIsModalOpen}
            subjectId={subjectId}
          />
        )}
      </div>
    </>
  );
};
