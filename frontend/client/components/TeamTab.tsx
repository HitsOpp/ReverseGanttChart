import { FiUserPlus, FiPlusCircle } from "react-icons/fi";

const mockTeams = [
  { id: 1, name: "Alpha Team", members: 5 },
  { id: 2, name: "Beta Team", members: 3 },
  { id: 3, name: "Gamma Team", members: 7 },
];

export const TeamTab = () => {
  return (
    <div className="bg-white shadow-sm rounded-lg overflow-hidden">
      {mockTeams.map((team, index) => (
        <div
          key={team.id}
          className={`
            p-4 flex justify-between items-center
            ${index !== mockTeams.length - 1 ? "border-b border-gray-200" : ""}
          `}
        >
          <div>
            <div className="text-lg font-medium">{team.name}</div>
            <div className="text-gray-500 text-sm">
              Участников: {team.members}
            </div>
          </div>

          <div className="flex gap-2">
            <button className="px-3 py-2 bg-blue-50 text-blue-700 border border-blue-200 rounded-lg flex gap-2 items-center hover:bg-blue-100 transition">
              <FiUserPlus />
              Присоединиться
            </button>

            <button className="px-3 py-2 bg-green-50 text-green-700 border border-green-200 rounded-lg flex gap-2 items-center hover:bg-green-100 transition">
              <FiPlusCircle />
              Добавить
            </button>
          </div>
        </div>
      ))}
    </div>
  );
};
