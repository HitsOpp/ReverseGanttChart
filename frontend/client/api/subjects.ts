import { apiCall } from "client/utils";
import { queryOptions } from "@tanstack/react-query";
import {
  type loadProfileDataResponse,
  type loadSubjectType,
  type loadProjectType,
  type ProjectTaskType,
} from "client/shared";

export const subjectKeyFactory = {
  loadSubject: () => ["loadSubject"],
  loadSubjectById: (subjectId: string) => [subjectId, "loadSubject"],
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
    queryKey: subjectKeyFactory.loadSubjectById(subjectId),
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
      apiCall.get<loadProfileDataResponse[]>(`/Subjects/${role}`, {
        params: { subjectId },
      }),
  });
};

export const loadSubjectProjects = (subjectId: string) => {
  return queryOptions({
    queryKey: ["subject-projects", subjectId],
    queryFn: () =>
      apiCall.get<loadProjectType[]>("/Projects/subject-projects", {
        params: { subjectId },
      }),
  });
};
export const loadProjectTasks = (projectId: string) => {
  return queryOptions({
    queryKey: ["project-tasks", projectId],
    queryFn: () =>
      apiCall.get<ProjectTaskType[]>("/tasks", {
        params: { projectId },
      }),
  });
};

export const createProject = (
  subjectId: string,
  data: {
    name: string;
    description: string;
    startDate: string;
    endDate: string;
  }
) => {
  return apiCall.post("/project/create", data, {
    params: { subjectId },
  });
};

export const editProject = (
  projectId: string,
  data: {
    name: string;
    description: string;
    startDate: string;
    endDate: string;
  }
) => {
  return apiCall.put("/project/edit", data, {
    params: { projectId },
  });
};

export const createSubject = (data: {
  name: string;
  description: string;
  color: string;
}) => {
  return apiCall.post("/Subjects/create", data);
};

export const joinSubject = (subjectId: string) => {
  return apiCall.post("/Subjects/join", { subjectId });
};

export const createTask = (
  projectId: string,
  data: {
    name: string;
    description: string;
    dueDate?: string;
    priority?: number;
  }
) => {
  return apiCall.post("/tasks/create", data, {
    params: { projectId },
  });
};

export const deleteProject = (projectId: string) => {
  return apiCall.delete("/project/delete", {
    params: { projectId },
  });
};

export const deleteTask = (taskId: string) => {
  return apiCall.delete("/tasks/delete", {
    params: { taskId },
  });
};

export const editTask = (
  taskId: string,
  data: {
    name: string;
    description: string;
    dueDate?: string;
    priority?: number;
  }
) => {
  return apiCall.put("/tasks/edit", data, {
    params: { taskId },
  });
};
