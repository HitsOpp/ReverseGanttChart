import { useState } from "react";
import { SubjectCard } from "client/components";
import { CreateSubject } from "client/api/subjects";

import { SubjectDetailPage } from "./SubjectDetailPage";

export const SubjectPage = () => {
  const [selectedSubject, setSelectedSubject] = useState<{
    title: string;
    institution: string;
    headerColor?: string;
    initialTab: "about" | "tasks" | "team" | "users";
  } | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [subjectCode, setSubjectCode] = useState("");
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
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
  const [isCreatingSubject, setIsCreatingSubject] = useState(false);

  const handleCreateSubject = async () => {
    if (!newSubjectTitle.trim()) {
      alert("Введите название предмета");
      return;
    }

    try {
      setIsCreatingSubject(true);
      await CreateSubject(
        newSubjectTitle,
        newSubjectDescription,
        newSubjectColor
      );

      setIsCreateModalOpen(false);
      setNewSubjectTitle("");
      setNewSubjectDescription("");
      setNewSubjectColor(colorOptions[0]);
    } catch (err) {
      console.error(err);
      alert("Не удалось создать предмет");
    } finally {
      setIsCreatingSubject(false);
    }
  };

  if (selectedSubject) {
    return <SubjectDetailPage {...selectedSubject} />;
  }

  return (
    <div className="min-h-screen">
      <div className="ml-15 h-[180px] bg-blue-300 pt-4">
        <div className="h-[75px] w-[1000px] mt-15 rounded-md ml-13 p-4 z-0 bg-white">
          <h3 className="text-3xl font-light text-gray-900 mb-8 z-100">
            Предметы
          </h3>
        </div>
      </div>

      <main className="lg:ml-10px p-8">
        <div className="max-w-7xl ml-5">
          <div className="flex justify-start ml-[10px] mb-6 gap-3">
            <div className="relative">
              <button
                onClick={() => {
                  setIsModalOpen(true);
                  setIsCreateModalOpen(false);
                }}
                className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg shadow transition"
              >
                Добавить предмет
              </button>
              {isModalOpen && (
                <div className="absolute top-full left-0 mt-2 z-50 w-full max-w-lg min-w-[360px]">
                  <div className="bg-white/95 backdrop-blur-sm w-full rounded-xl border border-gray-200 shadow-2xl ring-1 ring-blue-100 p-6">
                    <div className="flex justify-between items-center mb-4">
                      <h3 className="text-xl font-semibold text-gray-900">
                        Добавить предмет
                      </h3>
                      <button
                        onClick={() => setIsModalOpen(false)}
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
                          placeholder="например, MATH101"
                          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-400"
                        />
                      </div>

                      <div className="flex justify-end gap-3 pt-2">
                        <button
                          onClick={() => setIsModalOpen(false)}
                          className="px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-lg"
                        >
                          Отмена
                        </button>
                        <button
                          onClick={() => {
                            //отправить subjectCode на сервер
                            setIsModalOpen(false);
                            setSubjectCode("");
                            setIsCreateModalOpen(false);
                          }}
                          className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg shadow"
                        >
                          Добавить
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
                  setIsModalOpen(false);
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
                          disabled={isCreatingSubject}
                          className="px-4 py-2 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg shadow disabled:opacity-60"
                        >
                          {isCreatingSubject ? "Создание..." : "Создать"}
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              )}
            </div>
          </div>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-10">
            <SubjectCard
              customHeaderColor="#8B5CF6"
              title="Математика"
              institution="Высшая IT школа"
              onNavigate={(tab) =>
                setSelectedSubject({
                  title: "Математика",
                  institution: "Высшая IT школа",
                  headerColor: "#8B5CF6",
                  initialTab: tab,
                })
              }
            />
            <SubjectCard
              customHeaderColor="#10B981"
              title="Физика"
              institution="Высшая IT школа"
              onNavigate={(tab) =>
                setSelectedSubject({
                  title: "Физика",
                  institution: "Высшая IT школа",
                  headerColor: "#10B981",
                  initialTab: tab,
                })
              }
            />
            <SubjectCard
              customHeaderColor="#3B82F6"
              title="Информатика"
              institution="Высшая IT школа"
              onNavigate={(tab) =>
                setSelectedSubject({
                  title: "Информатика",
                  institution: "Высшая IT школа",
                  headerColor: "#3B82F6",
                  initialTab: tab,
                })
              }
            />
            <SubjectCard
              customHeaderColor="#EF4444"
              title="Химия"
              institution="Высшая IT школа"
              onNavigate={(tab) =>
                setSelectedSubject({
                  title: "Химия",
                  institution: "Высшая IT школа",
                  headerColor: "#EF4444",
                  initialTab: tab,
                })
              }
            />
            <SubjectCard
              customHeaderColor="#F59E0B"
              title="Биология"
              institution="Высшая IT школа"
              onNavigate={(tab) =>
                setSelectedSubject({
                  title: "Биология",
                  institution: "Высшая IT школа",
                  headerColor: "#F59E0B",
                  initialTab: tab,
                })
              }
            />
            <SubjectCard
              customHeaderColor="#EC4899"
              title="История"
              institution="Высшая IT школа"
              onNavigate={(tab) =>
                setSelectedSubject({
                  title: "История",
                  institution: "Высшая IT школа",
                  headerColor: "#EC4899",
                  initialTab: tab,
                })
              }
            />
            <SubjectCard
              customHeaderColor="#06B6D4"
              title="Литература"
              institution="Высшая IT школа"
              onNavigate={(tab) =>
                setSelectedSubject({
                  title: "Литература",
                  institution: "Высшая IT школа",
                  headerColor: "#06B6D4",
                  initialTab: tab,
                })
              }
            />
            <SubjectCard
              customHeaderColor="#8B5CF6"
              title="География"
              institution="Высшая IT школа"
              onNavigate={(tab) =>
                setSelectedSubject({
                  title: "География",
                  institution: "Высшая IT школа",
                  headerColor: "#8B5CF6",
                  initialTab: tab,
                })
              }
            />
          </div>
        </div>
      </main>
    </div>
  );
};
