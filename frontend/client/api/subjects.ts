import { apiCall } from "client/utils";
import { queryOptions } from "@tanstack/react-query";
import {
  type loadProfileDataResponse,
  type loadSubjectType,
} from "client/shared";

const subjectKeyFactory = {
  loadSubject: () => ["loadSubject"],
  loadPersonsInSubject: (subjectId: string, role: string) => [
    subjectId,
    role,
    "loadPersonsInSubject",
  ],
};

export const loadSubjects = () => {
  return queryOptions({
    queryKey: subjectKeyFactory.loadSubject(),
    queryFn: () => apiCall.get<loadSubjectType[]>("/Subjects/my-subjects"),
  });
};
export const loadSubjectById = (subjectId: string) => {
  return queryOptions({
    queryKey: subjectKeyFactory.loadSubject(),
    queryFn: () =>
      apiCall.get<loadSubjectType>(`/Subjects/information`, {
        params: { subjectId },
      }),
  });
};
export const loadPersonsInSubject = (
  subjectId: string,
  role: "teachers" | "students"
) => {
  return queryOptions({
    queryKey: subjectKeyFactory.loadPersonsInSubject(subjectId, role),
    queryFn: () =>
      apiCall.get<loadProfileDataResponse[]>(`/User/subject/${role}`, {
        params: { subjectId },
      }),
  });
};
export const CreateSubject = (name:string, description:string, color:string) => {
  return queryOptions({
    queryKey: subjectKeyFactory.loadSubject(),
    queryFn: () => apiCall.get<loadSubjectType[]>(`/subjects/create/`),
  });
};