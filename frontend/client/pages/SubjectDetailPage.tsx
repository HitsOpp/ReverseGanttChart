import { useState } from "react";

type InitialTab = "about" | "tasks" | "team" | "users";

interface SubjectDetailPageProps {
  title: string;
  institution: string;
  headerColor?: string;
  initialTab?: InitialTab;
}

export const SubjectDetailPage = ({
  title,
  institution,
  headerColor = "#3B82F6",
  initialTab = "about",
}: SubjectDetailPageProps) => {
  const [selectedTab, setSelectedTab] = useState(initialTab);

  return (
    <div className="min-h-screen bg-gray-100">
      <div className="w-full p-6" style={{ backgroundColor: headerColor }}>
        <h1 className="text-3xl font-bold text-white">{title}</h1>
        <p className="text-white opacity-80 mt-1">{institution}</p>
      </div>

      <div className="flex gap-4 p-4 bg-white shadow">
        {["about", "tasks", "team", "users"].map((tab) => (
          <button
            key={tab}
            onClick={() => setSelectedTab(tab)}
            className={`px-4 py-2 rounded-md font-medium ${
              selectedTab === tab
                ? "bg-blue-500 text-white"
                : "bg-gray-100 text-gray-700"
            }`}
          >
            {tab === "about"
              ? "О предмете"
              : tab === "tasks"
              ? "Задания"
              : tab === "team"
              ? "Команда"
              : "Пользователи"}
          </button>
        ))}
      </div>

      <div className="p-6">
        {selectedTab === "about" && <p>Описание предмета...</p>}
        {selectedTab === "tasks" && <p>Список заданий...</p>}
        {selectedTab === "team" && <p>Состав команды...</p>}
        {selectedTab === "users" && <p>Список пользователей...</p>}
      </div>
    </div>
  );
};
