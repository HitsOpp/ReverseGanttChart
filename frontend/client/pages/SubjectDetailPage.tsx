import { useParams, useNavigate } from "react-router-dom";
import { useQuery } from "@tanstack/react-query";
import { loadSubjectById } from "client/api";
import { FiCheckSquare, FiInfo, FiUser, FiUsers } from "react-icons/fi";
import { TeamTab, UsersTab } from "@/components";

export const SubjectDetailPage = () => {
  const navigate = useNavigate();
  const { id, tab } = useParams<{ id: string; tab?: string }>();

  const selectedTab = tab ?? "about";

  const { data: subject } = useQuery(loadSubjectById(id!));

  const tabs = [
    { id: "about", label: "О предмете", icon: <FiInfo className="w-5 h-5" /> },
    {
      id: "tasks",
      label: "Задания",
      icon: <FiCheckSquare className="w-5 h-5" />,
    },
    { id: "team", label: "Команда", icon: <FiUsers className="w-5 h-5" /> },
    {
      id: "users",
      label: "Пользователи",
      icon: <FiUser className="w-5 h-5" />,
    },
  ];

  if (!subject) return <p>Загрузка...</p>;

  return (
    <div className="min-h-screen bg-gray-100">
      <div
        className="w-full pt-10 pb-10 pl-5 flex flex-col justify-end items-start"
        style={{
          backgroundColor: subject.headerColor ?? "#3B82F6",
          minHeight: "220px",
        }}
      >
        <h1 className="text-5xl font-bold text-white">{subject.name}</h1>
        <p className="text-white opacity-90 mt-2 text-2xl">
          {subject.description}
        </p>
      </div>

      <div className="flex gap-3 p-4 bg-white shadow border-b border-gray-200">
        {tabs.map((t) => (
          <button
            key={t.id}
            onClick={() => navigate(`/subjects/${id}/${t.id}`)}
            className={`
              px-4 py-3 rounded-lg transition-all duration-200
              flex items-center gap-2 border
              ${
                selectedTab === t.id
                  ? "bg-blue-50 text-blue-700 border-blue-200"
                  : "bg-white hover:bg-gray-50 border-transparent text-gray-700"
              }
            `}
          >
            {t.icon}
            <span className="text-base font-light">{t.label}</span>
          </button>
        ))}
      </div>

      <div className="p-6 text-lg">
        {selectedTab === "about" && <p>{subject.description}</p>}
        {selectedTab === "tasks" && <p>Список заданий...</p>}
        {selectedTab === "team" && <TeamTab subjectId={id!} />}
        {selectedTab === "users" && <UsersTab subjectId={id!} />}
      </div>
    </div>
  );
};
