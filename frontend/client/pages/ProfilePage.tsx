import { useQuery } from "@tanstack/react-query";
import { loadProfileData } from "client/api/user";
import { useUpdateProfileName } from "client/hooks/useUpdateProfileName";
import { useState } from "react";

export const ProfilePage = () => {
  const { data } = useQuery(loadProfileData());
  const updateProfileName = useUpdateProfileName();
  const [isEditing, setIsEditing] = useState(false);
  const [fullName, setFullName] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    updateProfileName.mutate(fullName, {
      onSuccess: () => setIsEditing(false),
    });
  };

  return (
    <div className="max-w-lg mx-auto mt-10 bg-white shadow-md rounded-xl p-6 space-y-6">
      <h2 className="text-2xl font-semibold text-gray-800">
        Профиль пользователя
      </h2>

      {isEditing ? (
        <form onSubmit={handleSubmit} className="space-y-4">
          <label className="block">
            <span className="font-medium text-gray-700">Полное имя:</span>
            <input
              className="mt-1 w-full border border-gray-300 rounded-md px-3 py-2 focus:ring-2 focus:ring-blue-400 focus:outline-none"
              value={fullName}
              onChange={(e) => setFullName(e.target.value)}
            />
          </label>

          <div className="flex gap-3">
            <button
              type="submit"
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition"
            >
              Сохранить
            </button>

            <button
              type="button"
              className="px-4 py-2 bg-gray-200 rounded-md hover:bg-gray-300 transition"
              onClick={() => {
                setIsEditing(false);
                setFullName(data?.fullName || "");
              }}
            >
              Отмена
            </button>
          </div>
        </form>
      ) : (
        <div className="space-y-3">
          <p className="flex items-center justify-between">
            <span>
              <strong>Полное имя:</strong> {data?.fullName}
            </span>
            <button
              className="text-blue-600 hover:underline"
              onClick={() => {
                setFullName(data?.fullName || "");
                setIsEditing(true);
              }}
            >
              Изменить
            </button>
          </p>

          <p>
            <strong>Email:</strong> {data?.email}
          </p>

          <p>
            <strong>Роль:</strong>{" "}
            {data?.isTeacher ? "Преподаватель" : "Студент"}
          </p>
        </div>
      )}
    </div>
  );
};
