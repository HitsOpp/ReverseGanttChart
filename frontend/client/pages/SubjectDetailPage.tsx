import { useState } from "react";
import { FiInfo, FiCheckSquare, FiUsers, FiUser } from "react-icons/fi";
import { TeamTab, UsersTab } from "client/components";

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
  const [selectedTab, setSelectedTab] = useState<InitialTab>(initialTab);

  const tabs = [
    {
      id: "about",
      label: "О предмете",
      icon: <FiInfo className="w-5 h-5" />,
    },
    {
      id: "tasks",
      label: "Задания",
      icon: <FiCheckSquare className="w-5 h-5" />,
    },
    {
      id: "team",
      label: "Команда",
      icon: <FiUsers className="w-5 h-5" />,
    },
    {
      id: "users",
      label: "Пользователи",
      icon: <FiUser className="w-5 h-5" />,
    },
  ] as const;

  return (
    <>
      <div className="min-h-screen bg-gray-100">
        <div
          className="w-full pt-10 pb-10 pl-5 flex flex-col justify-end items-start"
          style={{ backgroundColor: headerColor, minHeight: "220px" }}
        >
          <h1 className="text-5xl font-bold text-white">{title}</h1>
          <p className="text-white opacity-90 mt-2 text-2xl">{institution}</p>
        </div>

        <div className="flex gap-3 p-4 bg-white shadow border-b border-gray-200">
          {tabs.map((tab) => (
            <button
              key={tab.id}
              onClick={() => setSelectedTab(tab.id as InitialTab)}
              className={`
              px-4 py-3 rounded-lg transition-all duration-200
              flex items-center gap-2 border
              ${
                selectedTab === tab.id
                  ? "bg-blue-50 text-blue-700 border-blue-200"
                  : "bg-white hover:bg-gray-50 border-transparent text-gray-700"
              }
          `}
            >
              {tab.icon}
              <span className="text-base font-light">{tab.label}</span>
            </button>
          ))}
        </div>

        <div className="p-6 text-lg">
          {selectedTab === "about" && <p>Описание предмета...</p>}
          {selectedTab === "tasks" && <p>Список заданий...</p>}
          {selectedTab === "team" && <TeamTab />}
          {selectedTab === "users" && <UsersTab />}
        </div>
      </div>
    </>
  );
};
