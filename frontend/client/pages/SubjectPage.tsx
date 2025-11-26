import { SubjectCard } from "client/components";
import { useQuery } from "@tanstack/react-query";
import { loadSubjects } from "client/api";
import { useNavigate } from "react-router";

export const SubjectPage = () => {
  const navigate = useNavigate();
  const { data } = useQuery(loadSubjects());
  console.log(data);
  return (
    <div className="min-h-screen">
      <div className="ml-17 h-[180px] bg-blue-300 pt-4">
        <div className="h-[75px] w-[1000px] mt-15 rounded-md ml-13 p-4 z-0 bg-white">
          <h3 className="text-3xl font-light text-gray-900 mb-8 z-100">
            Предметы
          </h3>
        </div>
      </div>

      <main className="lg:ml-10px p-8">
        <div className="max-w-7xl ml-5">
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-10">
            {data?.map((subject) => {
              return (
                <SubjectCard
                  key={subject.id}
                  customHeaderColor="#8B5CF6"
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
