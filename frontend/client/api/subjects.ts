import { apiCall } from "client/utils";
import { queryOptions } from "@tanstack/react-query";
import { type loadSubjectType } from "client/shared";

const subjectKeyFactory = {
  loadSubject: () => ["loadSubject"],
};

export const loadSubjects = () => {
  return queryOptions({
    queryKey: subjectKeyFactory.loadSubject(),
    queryFn: () => apiCall.get<loadSubjectType[]>("/subjects"),
  });
};
export const loadSubjectById = (id: string) => {
  return queryOptions({
    queryKey: subjectKeyFactory.loadSubject(),
    queryFn: () => apiCall.get<loadSubjectType[]>(`/subjects/${id}`),
  });
};
export const CreateSubject = (name:string, description:string, color:string) => {
  return queryOptions({
    queryKey: subjectKeyFactory.loadSubject(),
    queryFn: () => apiCall.get<loadSubjectType[]>(`/subjects/create/`),
  });
};