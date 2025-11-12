import { useState, type FC } from "react";

interface SidePanelProps {
  userName: string;
  userHref?: string;
}

type MenuItem = "subjects" | "support";

export const SidePanel: FC<SidePanelProps> = ({ userName, userHref = "#" }) => {
  const [isOpen, setIsOpen] = useState(true);
  const [activeItem, setActiveItem] = useState<MenuItem>("subjects");

  const togglePanel = () => {
    setIsOpen(!isOpen);
  };

  const handleItemClick = (item: MenuItem) => {
    setActiveItem(item);
  };

  const handleLogout = () => {
    console.log("Logout clicked");
  };

  return (
    <>
      {isOpen && (
        <div
          className="fixed inset-0 bg-black bg-opacity-50 z-40 lg:hidden"
          onClick={togglePanel}
        />
      )}

      <button
        onClick={togglePanel}
        className="fixed top-4 left-4 z-50 p-3 bg-white rounded-lg shadow-lg hover:shadow-xl transition-shadow border border-gray-200"
      >
        <svg
          className="w-6 h-6 text-gray-700"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth={2}
            d="M4 6h16M4 12h16M4 18h16"
          />
        </svg>
      </button>

      <div
        className={`
        fixed top-0 left-0 h-full bg-white shadow-xl z-50
        transform transition-transform duration-300 ease-in-out
        ${isOpen ? "translate-x-0" : "-translate-x-full"}
        w-80 flex flex-col
      `}
      >
        <div className="p-6 border-b border-gray-200">
          <div className="flex items-center justify-between mb-4">
            <h1 className="text-2xl font-bold text-gray-900">Students Tasks</h1>
            <button
              onClick={togglePanel}
              className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
            >
              <svg
                className="w-5 h-5 text-gray-500"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M6 18L18 6M6 6l12 12"
                />
              </svg>
            </button>
          </div>

          <div className="text-sm text-gray-500 mb-4">Русский</div>

          <div className="space-y-3">
            <button
              onClick={() => handleItemClick("subjects")}
              className={`
                w-full text-left p-4 rounded-lg border-2 transition-all duration-200
                ${
                  activeItem === "subjects"
                    ? "border-blue-500 bg-blue-50 text-blue-700"
                    : "border-gray-200 hover:border-gray-300 hover:bg-gray-50"
                }
              `}
            >
              <div className="font-medium text-lg">Предметы</div>
            </button>

            <button
              onClick={() => handleItemClick("support")}
              className={`
                w-full text-left p-4 rounded-lg border-2 transition-all duration-200
                ${
                  activeItem === "support"
                    ? "border-blue-500 bg-blue-50 text-blue-700"
                    : "border-gray-200 hover:border-gray-300 hover:bg-gray-50"
                }
              `}
            >
              <div className="font-medium text-lg">Поддержка</div>
            </button>
          </div>
        </div>

        <div className="flex-1"></div>

        <div className="p-6 border-t border-gray-200 bg-gray-50">
          <a
            href={userHref}
            className="block py-3 px-4 bg-white rounded-lg border border-gray-200 hover:border-gray-300 transition-colors mb-3"
          >
            <div className="font-medium text-gray-900 text-center">
              {userName}
            </div>
          </a>

          <button
            onClick={handleLogout}
            className="w-full py-3 px-4 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-colors font-medium"
          >
            Выйти
          </button>
        </div>
      </div>
    </>
  );
};
