import { useState } from "react";
import { SubjectCard } from "client/components";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  loadSubjects,
  createSubject,
  joinSubject,
  subjectKeyFactory,
} from "client/api";
import { useNavigate } from "react-router";

export const SubjectPage = () => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { data } = useQuery(loadSubjects());

  const [isJoinModalOpen, setIsJoinModalOpen] = useState(false);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [subjectCode, setSubjectCode] = useState("");

  const colorOptions = [
    "#8B5CF6",
    "#10B981",
    "#3B82F6",
    "#F59E0B",
    "#EF4444",
    "#06B6D4",
    "#EC4899",
  ];

  const [newSubjectTitle, setNewSubjectTitle] = useState("");
  const [newSubjectDescription, setNewSubjectDescription] = useState("");
  const [newSubjectColor, setNewSubjectColor] = useState(colorOptions[0]);

  const joinMutation = useMutation({
    mutationFn: (code: string) => joinSubject(code),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: subjectKeyFactory.loadSubject(),
      });
      setIsJoinModalOpen(false);
      setSubjectCode("");
    },
  });

  const createMutation = useMutation({
    mutationFn: (payload: {
      name: string;
      description: string;
      color: string;
    }) => createSubject(payload),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: subjectKeyFactory.loadSubject(),
      });
      setIsCreateModalOpen(false);
      setNewSubjectTitle("");
      setNewSubjectDescription("");
      setNewSubjectColor(colorOptions[0]);
    },
  });

  const handleJoinSubject = () => {
    if (!subjectCode.trim()) {
      alert("Введите код предмета");
      return;
    }

    joinMutation.mutate(subjectCode);
  };

  const handleCreateSubject = () => {
    if (!newSubjectTitle.trim()) {
      alert("Введите название предмета");
      return;
    }

    createMutation.mutate({
      name: newSubjectTitle,
      description: newSubjectDescription,
      color: newSubjectColor,
    });
  };

  return (
    <div className="min-h-screen">
      <div className="h-[180px] bg-blue-300 pt-4">
        <div className="h-[75px] w-[1000px] mt-15 rounded-md ml-10 p-4 z-0 bg-white">
          <h3 className="text-3xl font-light text-gray-900 mb-8 z-100">
            Предметы
          </h3>
        </div>
      </div>

      <main className="lg:ml-10px pt-5 pr-4 p-2">
        <div className="max-w-7xl ml-2">
          <div className="flex justify-start mb-6 gap-3">
            <div className="relative">
              <button
                onClick={() => {
                  setIsJoinModalOpen(true);
                  setIsCreateModalOpen(false);
                }}
                className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg shadow transition"
              >
                Добавить предмет
              </button>
              {isJoinModalOpen && (
                <div className="absolute top-full left-0 mt-2 z-50 w-full max-w-lg min-w-[360px]">
                  <div className="bg-white/95 backdrop-blur-sm w-full rounded-xl border border-gray-200 shadow-2xl ring-1 ring-blue-100 p-6">
                    <div className="flex justify-between items-center mb-4">
                      <h3 className="text-xl font-semibold text-gray-900">
                        Добавить предмет
                      </h3>
                      <button
                        onClick={() => setIsJoinModalOpen(false)}
                        className="text-gray-500 hover:text-gray-700"
                      >
                        ✕
                      </button>
                    </div>

                    <div className="space-y-4">
                      <div>
                        <label className="block text-sm text-gray-700 mb-1">
                          Код предмета
                        </label>
                        <input
                          value={subjectCode}
                          onChange={(e) => setSubjectCode(e.target.value)}
                          placeholder="например, 3fa85f64-5717-4562-b3fc-2c963f66afa6"
                          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-400"
                        />
                      </div>

                      <div className="flex justify-end gap-3 pt-2">
                        <button
                          onClick={() => setIsJoinModalOpen(false)}
                          className="px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-lg"
                        >
                          Отмена
                        </button>
                        <button
                          onClick={handleJoinSubject}
                          disabled={joinMutation.status === "pending"}
                          className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg shadow disabled:opacity-60"
                        >
                          {joinMutation.status === "pending"
                            ? "Присоединение..."
                            : "Добавить"}
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              )}
            </div>

            <div className="relative">
              <button
                onClick={() => {
                  setIsCreateModalOpen(true);
                  setIsJoinModalOpen(false);
                }}
                className="px-4 py-2 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg shadow transition"
              >
                Создать предмет
              </button>
              {isCreateModalOpen && (
                <div className="absolute top-full left-0 mt-2 z-50 w-full max-w-lg min-w-[360px]">
                  <div className="bg-white/95 backdrop-blur-sm w-full rounded-xl border border-gray-200 shadow-2xl ring-1 ring-emerald-100 p-6">
                    <div className="flex justify-between items-center mb-4">
                      <h3 className="text-xl font-semibold text-gray-900">
                        Создать предмет
                      </h3>
                      <button
                        onClick={() => setIsCreateModalOpen(false)}
                        className="text-gray-500 hover:text-gray-700"
                      >
                        ✕
                      </button>
                    </div>

                    <div className="space-y-4">
                      <div>
                        <label className="block text-sm text-gray-700 mb-1">
                          Название
                        </label>
                        <input
                          value={newSubjectTitle}
                          onChange={(e) => setNewSubjectTitle(e.target.value)}
                          placeholder="Например, Анализ данных"
                          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-400"
                        />
                      </div>

                      <div>
                        <label className="block text-sm text-gray-700 mb-1">
                          Описание
                        </label>
                        <textarea
                          value={newSubjectDescription}
                          onChange={(e) =>
                            setNewSubjectDescription(e.target.value)
                          }
                          placeholder="Краткое описание курса"
                          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-emerald-400"
                          rows={3}
                        />
                      </div>

                      <div>
                        <label className="block text-sm text-gray-700 mb-1">
                          Цвет предмета
                        </label>
                        <div className="flex flex-wrap gap-2">
                          {colorOptions.map((color) => {
                            const isActive = newSubjectColor === color;
                            return (
                              <button
                                key={color}
                                type="button"
                                onClick={() => setNewSubjectColor(color)}
                                className={`h-10 w-10 rounded-full border transition ${
                                  isActive
                                    ? "ring-2 ring-emerald-400 border-emerald-300"
                                    : "border-gray-200 hover:border-emerald-200"
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
                          onClick={() => setIsCreateModalOpen(false)}
                          className="px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-lg"
                        >
                          Отмена
                        </button>
                        <button
                          onClick={handleCreateSubject}
                          disabled={createMutation.status === "pending"}
                          className="px-4 py-2 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg shadow disabled:opacity-60"
                        >
                          {createMutation.status === "pending"
                            ? "Создание..."
                            : "Создать"}
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              )}
            </div>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-10">
            {data?.map((subject) => {
              const cardColor =
                subject.headerColor ?? subject.color ?? "#8B5CF6";

              return (
                <SubjectCard
                  key={subject.id}
                  customHeaderColor={cardColor}
                  title={subject.name}
                  institution={subject.description}
                  onNavigate={(tab) =>
                    navigate(`/subjects/${subject.id}/${tab}`)
                  }
                />
              );
            })}
          </div>
        </div>
      </main>
    </div>
  );
};
