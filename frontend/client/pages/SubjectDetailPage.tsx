import { useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { deleteSubject, editSubject, loadSubjectById, subjectKeyFactory } from "client/api";
import { useProfile } from "@/hooks/useProfile";
import {
  FiCheckSquare,
  FiInfo,
  FiUser,
  FiUsers,
  FiCopy,
  FiEdit2,
  FiTrash2,
} from "react-icons/fi";
import { TeamTab, UsersTab, TasksTab } from "client/components";

export const SubjectDetailPage = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { id, tab } = useParams<{ id: string; tab?: string }>();
  const { data: profile } = useProfile();
  const [copied, setCopied] = useState(false);
  const [isEditOpen, setIsEditOpen] = useState(false);
  const [editName, setEditName] = useState("");
  const [editDescription, setEditDescription] = useState("");
  const [editColor, setEditColor] = useState("#3B82F6");

  const selectedTab = tab ?? "about";

  const { data: subject } = useQuery(loadSubjectById(id!));

  const colorOptions = [
    "#8B5CF6",
    "#10B981",
    "#3B82F6",
    "#F59E0B",
    "#EF4444",
    "#06B6D4",
    "#EC4899",
  ];

  const deleteMutation = useMutation({
    mutationFn: () => deleteSubject(id!),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: subjectKeyFactory.loadSubject(),
      });
      navigate("/subjects");
    },
  });

  const editMutation = useMutation({
    mutationFn: () =>
      editSubject(id!, {
        name: editName,
        description: editDescription,
        color: editColor,
      }),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: loadSubjectById(id!).queryKey,
      });
      setIsEditOpen(false);
    },
  });

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

  const headerColor = subject.headerColor ?? subject.color ?? "#3B82F6";
  const isTeacher =
    (profile?.isTeacher && profile.isTeacher) ||
    subject.currentUserRole === "teacher";

  const openEdit = () => {
    setEditName(subject.name);
    setEditDescription(subject.description);
    setEditColor(subject.color ?? "#3B82F6");
    setIsEditOpen(true);
  };

  return (
    <div className="min-h-screen bg-gray-100">
      <div
        className="w-full pt-10 pb-10 pl-5 flex flex-col justify-end items-start"
        style={{
          backgroundColor: headerColor,
          minHeight: "220px",
        }}
      >
        <h1 className="text-5xl font-bold text-white">{subject.name}</h1>
        <p className="text-white/90 mt-1 text-xl">Высшая IT школа</p>
      </div>

      <div className="flex items-center justify-between p-4 bg-white shadow border-b border-gray-200">
        <div className="flex gap-3">
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

        {isTeacher && (
          <div className="flex items-center gap-2">
            <button
              onClick={openEdit}
              className="inline-flex items-center gap-1 px-3 py-1.5 rounded-md border border-gray-300 bg-white text-gray-700 hover:border-blue-300 hover:text-blue-700 hover:bg-gray-50 text-xs transition"
              aria-label="Редактировать предмет"
            >
              <FiEdit2 className="w-4 h-4" />
              <span>Редактировать</span>
            </button>
            <button
              onClick={() => {
                deleteMutation.mutate();
              }}
              disabled={deleteMutation.isPending}
              className="inline-flex items-center gap-1 px-3 py-1.5 rounded-md border border-gray-300 bg-white text-gray-700 hover:border-red-300 hover:text-red-600 hover:bg-gray-50 text-xs transition disabled:opacity-60"
              aria-label="Удалить предмет"
            >
              <FiTrash2 className="w-4 h-4" />
              <span>Удалить</span>
            </button>
          </div>
        )}
      </div>

      <div className="p-6 text-lg">
        {selectedTab === "about" && (
          <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-8 space-y-5">
            <div>
              <h2 className="text-2xl font-semibold mb-3">О предмете</h2>
              <p className="text-lg text-gray-700 leading-relaxed">
                {subject.description}
              </p>
            </div>

            <div className="pt-2">
              <span className="block text-xs uppercase tracking-wide text-gray-500 mb-1">
                Код предмета
              </span>
              <button
                type="button"
                onClick={async () => {
                  try {
                    await navigator.clipboard?.writeText(subject.id);
                    setCopied(true);
                    setTimeout(() => setCopied(false), 1500);
                  } catch {
                    // ignore clipboard errors
                  }
                }}
                title="Скопировать код предмета"
                className="inline-flex items-center justify-between gap-3 px-4 py-3 rounded-lg bg-gray-50 border border-gray-200 shadow-sm cursor-pointer hover:border-blue-200 hover:bg-white hover:shadow-md transition"
              >
                <div className="flex flex-col items-start">
                  <span className="font-mono text-base text-gray-800">
                    {subject.id}
                  </span>
                  <span
                    className={`mt-1 text-xs ${
                      copied ? "text-green-600" : "text-gray-400"
                    }`}
                  >
                    {copied ? "Код скопирован" : "Нажмите, чтобы скопировать"}
                  </span>
                </div>
                <FiCopy
                  className={`w-4 h-4 ${
                    copied ? "text-green-600" : "text-gray-400"
                  }`}
                />
              </button>
            </div>
          </div>
        )}
        {selectedTab === "tasks" && <TasksTab subjectId={id!} />}
        {selectedTab === "team" && <TeamTab subjectId={id!} />}
        {selectedTab === "users" && <UsersTab subjectId={id!} />}
      </div>

      {isTeacher && isEditOpen && (
        <div className="fixed inset-0 flex p-8 items-center justify-center z-50">
          <div
            className="absolute inset-0 bg-black/20 backdrop-blur-sm"
            onClick={() => setIsEditOpen(false)}
          />

          <div
            className="bg-white rounded-2xl shadow-2xl w-full max-w-lg px-8 py-7 relative z-10"
            onClick={(e) => e.stopPropagation()}
          >
            <h2 className="text-2xl font-semibold mb-6">Редактировать предмет</h2>

            <div className="space-y-5">
              <input
                type="text"
                value={editName}
                onChange={(e) => setEditName(e.target.value)}
                placeholder="Название предмета"
                className="w-full border border-gray-300 rounded-lg px-4 py-3"
              />

              <textarea
                value={editDescription}
                onChange={(e) => setEditDescription(e.target.value)}
                placeholder="Описание"
                className="w-full border border-gray-300 rounded-lg px-4 py-3 min-h-[120px]"
              />

              <div>
              <label className="block text-sm text-gray-700 mb-3">
                Цвет предмета
              </label>
              <div className="flex flex-wrap gap-2">
                {colorOptions.map((color) => {
                  const isActive = editColor === color;
                  return (
                    <button
                      key={color}
                      type="button"
                      onClick={() => setEditColor(color)}
                      className={`h-10 w-10 rounded-full border transition ${
                        isActive
                          ? "ring-2 ring-blue-400 border-blue-300"
                          : "border-gray-200 hover:border-blue-200"
                      }`}
                      style={{ backgroundColor: color }}
                      aria-label={`Выбрать цвет ${color}`}
                    />
                  );
                })}
              </div>
              </div>

              <div className="flex justify-end gap-3 pt-2">
                <button
                  onClick={() => setIsEditOpen(false)}
                  className="px-4 py-2 rounded-lg border border-gray-300 hover:bg-gray-100 transition"
                >
                  Отмена
                </button>
                <button
                  onClick={() => editMutation.mutate()}
                  disabled={editMutation.isPending || !editName.trim()}
                  className={`px-5 py-2.5 rounded-lg text-white transition ${
                    editMutation.isPending || !editName.trim()
                      ? "bg-gray-400 cursor-not-allowed"
                      : "bg-blue-600 hover:bg-blue-700"
                  }`}
                >
                  {editMutation.isPending ? "Сохранение..." : "Сохранить"}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
