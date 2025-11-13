import { SidePanel, SubjectCard } from "client/components";

export const SubjectPage = () => {
  return (
    <div className="min-h-screen bg-yellow">
      <SidePanel userName="Мосин Денис Юрьевич" />
      <div className="ml-80 h-[180px] bg-blue-300 pt-4">
        <div className="h-[75px] w-[1000px] mt-15 rounded-md ml-13  p-4 z-0 bg-white">
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
            />
            <SubjectCard
              customHeaderColor="#10B981"
              title="Физика"
              institution="Высшая IT школа"
            />
            <SubjectCard
              customHeaderColor="#3B82F6"
              title="Информатика"
              institution="Высшая IT школа"
            />
            <SubjectCard
              customHeaderColor="#EF4444"
              title="Химия"
              institution="Высшая IT школа"
            />
            <SubjectCard
              customHeaderColor="#F59E0B"
              title="Биология"
              institution="Высшая IT школа"
            />
            <SubjectCard
              customHeaderColor="#EC4899"
              title="История"
              institution="Высшая IT школа"
            />
            <SubjectCard
              customHeaderColor="#06B6D4"
              title="Литература"
              institution="Высшая IT школа"
            />
            <SubjectCard
              customHeaderColor="#8B5CF6"
              title="География"
              institution="Высшая IT школа"
            />
          </div>
        </div>
      </main>
    </div>
  );
};
