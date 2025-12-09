import { useState } from "react";
import { FiUser, FiUsers } from "react-icons/fi";
import { useQuery } from "@tanstack/react-query";
import { loadPersonsInSubject } from "client/api";

interface UsersTabProps {
  subjectId: string;
}

export const UsersTab = ({ subjectId }: UsersTabProps) => {
  const [selectedGroup, setSelectedGroup] = useState<"students" | "teachers">(
    "teachers"
  );

  const { data, isLoading, isError } = useQuery(
    loadPersonsInSubject(subjectId, selectedGroup)
  );

  return (
    <>
      <div className="bg-white shadow-sm rounded-lg overflow-hidden p-6 flex justify-between items-center">
        <h1 className="text-4xl font-normal">Пользователи</h1>

        <div className="flex items-center bg-gray-200 rounded-md p-1">
          <button
            onClick={() => setSelectedGroup("students")}
            className={`
              flex items-center gap-1 px-4 py-2 text-sm transition-colors
              rounded-md
              ${
                selectedGroup === "students"
                  ? "bg-white"
                  : "bg-transparent text-gray-700"
              }
            `}
          >
            <FiUsers className="w-4 h-4" />
            Студенты
          </button>
          <button
            onClick={() => setSelectedGroup("teachers")}
            className={`
              flex items-center gap-1 px-4 py-2 text-sm transition-colors
              rounded-md
              ${
                selectedGroup === "teachers"
                  ? "bg-white"
                  : "bg-transparent text-gray-700"
              }
            `}
          >
            <FiUser className="w-4 h-4" />
            Преподаватели
          </button>
        </div>
      </div>

      <div className="mt-5 bg-white shadow-sm rounded-lg overflow-hidden">
        {isLoading && <p className="p-4 text-gray-500">Загрузка...</p>}
        {isError && (
          <p className="p-4 text-red-500">Ошибка при загрузке данных</p>
        )}

        {data?.map((user, index) => (
          <div
            key={user.id ?? index}
            className={`
              p-4 flex justify-between items-center
              ${index !== data.length - 1 ? "border-b border-gray-200" : ""}
            `}
          >
            <div>
              <div className="font-medium text-lg">{user.fullName}</div>
              <div className="text-gray-500 text-sm">
                {selectedGroup === "teachers" ? "Преподаватель" : "Студент"}
              </div>
            </div>
          </div>
        ))}
      </div>
    </>
  );
};
