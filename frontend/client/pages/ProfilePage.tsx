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
    <div className="min-h-screen bg-gray-100 flex justify-start pt-12">
      <div className="w-full ml-10 max-w-2xl space-y-6">
        <div className="bg-white shadow-sm rounded-lg p-6">
          <h1 className="text-4xl font-normal">Профиль</h1>
          <p className="text-gray-500 mt-3">Управление персональными данными</p>
        </div>

        <div className="bg-white shadow-sm rounded-lg p-6">
          {isEditing ? (
            <form onSubmit={handleSubmit} className="space-y-6">
              <div>
                <label className="block text-sm text-gray-600 mb-1">
                  Полное имя
                </label>
                <input
                  className="w-full border border-gray-300 rounded-lg px-4 py-3 text-lg focus:ring-2 focus:ring-blue-500 focus:outline-none transition"
                  value={fullName}
                  onChange={(e) => setFullName(e.target.value)}
                />
              </div>

              <div className="flex gap-3">
                <button
                  type="submit"
                  className="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
                >
                  Сохранить
                </button>

                <button
                  type="button"
                  className="px-6 py-3 bg-gray-200 rounded-lg hover:bg-gray-300 transition"
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
            <div className="space-y-5 text-lg">
              <div className="flex justify-between items-center border-b border-gray-200 pb-3">
                <div>
                  <div className="text-gray-500 text-sm">Полное имя</div>
                  <div className="font-medium text-xl">{data?.fullName}</div>
                </div>

                <button
                  className="inline-flex items-center px-4 py-2 text-sm rounded-lg border border-blue-200 text-blue-700 bg-blue-50 hover:bg-blue-600 hover:text-white hover:border-blue-600 transition"
                  onClick={() => {
                    setFullName(data?.fullName || "");
                    setIsEditing(true);
                  }}
                >
                  Изменить
                </button>
              </div>

              <div className="border-b border-gray-200 pb-3">
                <div className="text-gray-500 text-sm">Email</div>
                <div className="font-medium text-xl">{data?.email}</div>
              </div>

              <div>
                <div className="text-gray-500 text-sm">Роль</div>
                <div className="font-medium text-xl">
                  {data?.isTeacher ? "Преподаватель" : "Студент"}
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
