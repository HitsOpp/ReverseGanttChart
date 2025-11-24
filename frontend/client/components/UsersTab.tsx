const mockUsers = [
  { id: 1, name: "Иван Иванов", role: "Студент" },
  { id: 2, name: "Петр Петров", role: "Студент" },
  { id: 3, name: "Анна Смирнова", role: "Преподаватель" },
];

export const UsersTab = () => {
  return (
    <div className="bg-white shadow-sm rounded-lg overflow-hidden">
      {mockUsers.map((user, index) => (
        <div
          key={user.id}
          className={`
            p-4 flex justify-between items-center
            ${index !== mockUsers.length - 1 ? "border-b border-gray-200" : ""}
          `}
        >
          <div>
            <div className="font-medium text-lg">{user.name}</div>
            <div className="text-gray-500 text-sm">{user.role}</div>
          </div>
        </div>
      ))}
    </div>
  );
};
