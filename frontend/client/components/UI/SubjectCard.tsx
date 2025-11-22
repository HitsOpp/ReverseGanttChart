import { type FC } from "react";

interface SubjectCardProps {
  title: string;
  institution: string;
  className?: string;
  headerColor?:
    | "blue"
    | "green"
    | "red"
    | "purple"
    | "orange"
    | "indigo"
    | "pink";
  customHeaderColor?: string;
  onNavigate?: (tab: "about" | "tasks" | "team" | "users") => void;
}

export const SubjectCard: FC<SubjectCardProps> = ({
  title,
  institution,
  className = "",
  headerColor = "blue",
  customHeaderColor,
  onNavigate,
}) => {
  const colorSchemes: Record<string, { header: string; text: string }> = {
    blue: {
      header: "bg-gradient-to-r from-blue-600 to-blue-700",
      text: "text-blue-100",
    },
    green: {
      header: "bg-gradient-to-r from-green-600 to-green-700",
      text: "text-green-100",
    },
    red: {
      header: "bg-gradient-to-r from-red-600 to-red-700",
      text: "text-red-100",
    },
    purple: {
      header: "bg-gradient-to-r from-purple-600 to-purple-700",
      text: "text-purple-100",
    },
    orange: {
      header: "bg-gradient-to-r from-orange-600 to-orange-700",
      text: "text-orange-100",
    },
    indigo: {
      header: "bg-gradient-to-r from-indigo-600 to-indigo-700",
      text: "text-indigo-100",
    },
    pink: {
      header: "bg-gradient-to-r from-pink-600 to-pink-700",
      text: "text-pink-100",
    },
  };

  const selectedScheme = colorSchemes[headerColor];
  const headerStyle = customHeaderColor
    ? { backgroundColor: customHeaderColor }
    : {};
  const headerClass = customHeaderColor
    ? "p-4 text-white"
    : `p-4 text-white ${selectedScheme.header}`;

  return (
    <div
      className={`bg-white rounded-2xl shadow-lg border border-gray-200 overflow-hidden w-[272px] h-[316px] ${className}`}
    >
      <div className={headerClass} style={headerStyle}>
        <h3 className="font-bold text-lg leading-tight mb-1">{title}</h3>
        <p
          className={`text-sm opacity-90 ${
            customHeaderColor ? "text-white opacity-80" : selectedScheme.text
          }`}
        >
          {institution}
        </p>
      </div>

      <div className="p-4 space-y-3">
        <div
          className="flex items-center gap-3 p-3 rounded-lg border border-gray-200 hover:border-blue-300 hover:bg-blue-50 transition-colors cursor-pointer group"
          onClick={() => onNavigate?.("users")}
        >
          <div className="w-8 h-8 bg-blue-100 rounded-lg flex items-center justify-center group-hover:bg-blue-200 transition-colors">
            <svg
              className="w-4 h-4 text-blue-600"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z"
              />
            </svg>
          </div>
          <span className="font-medium text-gray-700">Пользователи</span>
        </div>

        <div
          className="flex items-center gap-3 p-3 rounded-lg border border-gray-200 hover:border-green-300 hover:bg-green-50 transition-colors cursor-pointer group"
          onClick={() => onNavigate?.("team")}
        >
          <div className="w-8 h-8 bg-green-100 rounded-lg flex items-center justify-center group-hover:bg-green-200 transition-colors">
            <svg
              className="w-4 h-4 text-green-600"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"
              />
            </svg>
          </div>
          <span className="font-medium text-gray-700">Команда</span>
        </div>

        <div
          className="flex items-center gap-3 p-3 rounded-lg border border-gray-200 hover:border-orange-300 hover:bg-orange-50 transition-colors cursor-pointer group"
          onClick={() => onNavigate?.("tasks")}
        >
          <div className="w-8 h-8 bg-orange-100 rounded-lg flex items-center justify-center group-hover:bg-orange-200 transition-colors">
            <svg
              className="w-4 h-4 text-orange-600"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01"
              />
            </svg>
          </div>
          <span className="font-medium text-gray-700">Задачи</span>
        </div>
      </div>
    </div>
  );
};
