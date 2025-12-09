import { useState } from "react";
import { FiUser, FiUsers } from "react-icons/fi";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  grantAssist,
  loadAssistsInSubject,
  loadPersonsInSubject,
  revokeAssist,
  subjectKeyFactory,
} from "client/api";
import { useProfile } from "@/hooks";

interface UsersTabProps {
  subjectId: string;
}

export const UsersTab = ({ subjectId }: UsersTabProps) => {
  const [selectedGroup, setSelectedGroup] = useState<"students" | "teachers">(
    "teachers"
  );

  const {
    data: students,
    isLoading: isStudentsLoading,
    isError: isStudentsError,
  } = useQuery(loadPersonsInSubject(subjectId, "students"));

  const {
    data: teachers,
    isLoading: isTeachersLoading,
    isError: isTeachersError,
  } = useQuery(loadPersonsInSubject(subjectId, "teachers"));

  const {
    data: assists,
    isLoading: isAssistsLoading,
    isError: isAssistsError,
  } = useQuery(loadAssistsInSubject(subjectId));

  const { data: profile } = useProfile();
  const isTeacher = profile?.isTeacher;

  const queryClient = useQueryClient();

  const grantAssistMutation = useMutation({
    mutationFn: (userId: string) => grantAssist(subjectId, userId),
    onSuccess: async () => {
      await queryClient.refetchQueries({
        queryKey: subjectKeyFactory.loadPersonsInSubject(subjectId, "students"),
      });
      await queryClient.refetchQueries({
        queryKey: subjectKeyFactory.loadAssistsInSubject(subjectId),
      });
      await queryClient.refetchQueries({
        queryKey: subjectKeyFactory.loadPersonsInSubject(subjectId, "teachers"),
      });
    },
  });

  const revokeAssistMutation = useMutation({
    mutationFn: (userId: string) => revokeAssist(subjectId, userId),
    onSuccess: async () => {
      await queryClient.refetchQueries({
        queryKey: subjectKeyFactory.loadAssistsInSubject(subjectId),
      });
      await queryClient.refetchQueries({
        queryKey: subjectKeyFactory.loadPersonsInSubject(subjectId, "students"),
      });
      await queryClient.refetchQueries({
        queryKey: subjectKeyFactory.loadPersonsInSubject(subjectId, "teachers"),
      });
    },
  });

  const isLoading =
    selectedGroup === "students"
      ? isStudentsLoading
      : isTeachersLoading || isAssistsLoading;

  const isError =
    selectedGroup === "students"
      ? isStudentsError
      : isTeachersError || isAssistsError;

  const usersToDisplay =
    selectedGroup === "students"
      ? (students ?? []).map((user) => ({
          ...user,
          roleLabel: "Студент" as const,
          isAssistant: false,
        }))
      : [
          ...(teachers ?? []).map((user) => ({
            ...user,
            roleLabel: "Преподаватель" as const,
            isAssistant: false,
          })),
          ...(assists ?? []).map((user) => ({
            ...user,
            roleLabel: "Ассистент" as const,
            isAssistant: true,
          })),
        ];

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

        {usersToDisplay.map((user, index) => (
          <div
            key={user.userId ?? user.id ?? index}
            className={`
              p-4 flex justify-between items-center
              ${
                index !== usersToDisplay.length - 1
                  ? "border-b border-gray-200"
                  : ""
              }
            `}
          >
            <div>
              <div className="font-medium text-lg">{user.fullName}</div>
              <div className="text-gray-500 text-sm">
                {user.roleLabel}
              </div>
            </div>
            {isTeacher && selectedGroup === "students" && (
              <button
                type="button"
                onClick={() =>
                  grantAssistMutation.mutate(user.userId ?? user.id)
                }
                disabled={grantAssistMutation.isPending}
                className={`
                  px-4 py-2 rounded-md text-sm font-medium transition
                  ${
                    grantAssistMutation.isPending
                      ? "bg-gray-300 text-gray-600 cursor-not-allowed"
                      : "bg-blue-600 text-white hover:bg-blue-700"
                  }
                `}
              >
                {grantAssistMutation.isPending
                  ? "Назначение..."
                  : "Назначить ассистентом"}
              </button>
            )}
            {isTeacher && selectedGroup === "teachers" && user.isAssistant && (
              <button
                type="button"
                onClick={() =>
                  revokeAssistMutation.mutate(user.userId ?? user.id)
                }
                disabled={revokeAssistMutation.isPending}
                className={`
                  px-4 py-2 rounded-md text-sm font-medium transition
                  ${
                    revokeAssistMutation.isPending
                      ? "bg-gray-300 text-gray-600 cursor-not-allowed"
                      : "bg-red-600 text-white hover:bg-red-700"
                  }
                `}
              >
                {revokeAssistMutation.isPending
                  ? "Удаление роли..."
                  : "Убрать роль ассистента"}
              </button>
            )}
          </div>
        ))}
      </div>
    </>
  );
};
