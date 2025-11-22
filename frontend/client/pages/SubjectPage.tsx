import { useState } from "react";
import { SidePanel, SubjectCard } from "client/components";
import { SubjectDetailPage } from "./SubjectDetailPage";

export const SubjectPage = () => {
  const [selectedSubject, setSelectedSubject] = useState<{
    title: string;
    institution: string;
    headerColor?: string;
    initialTab: "about" | "tasks" | "team" | "users";
  } | null>(null);

  if (selectedSubject) {
    return <SubjectDetailPage {...selectedSubject} />;
  }

  return (
    <div className="min-h-screen">
      <SidePanel userName="Мосин Денис Юрьевич" />

      <div className="ml-80 h-[180px] bg-blue-300 pt-4">
        <div className="h-[75px] w-[1000px] mt-15 rounded-md ml-13 p-4 z-0 bg-white">
          <h3 className="text-3xl font-light text-gray-900 mb-8 z-100">
            Предметы
          </h3>
        </div>
      </div>

      <main className="lg:ml-10px p-8">
        <div className="max-w-7xl mx-auto">
          <div className="ml-40 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-15">
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
