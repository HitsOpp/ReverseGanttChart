import { apiCall } from "client/utils";
import { mutationOptions, queryOptions } from "@tanstack/react-query";
import { type LoadAllTeams, type loadSubjectType } from "client/shared";

type TeamMethod = "edit" | "join" | "leave" | "create";
export const teamsKeyFactory = {
  loadAllTeams: (subjectId: string) => [subjectId, "teams"],
  actionsToTeam: (method: TeamMethod) => [method, "actionsToTeam"],
};

export const loadAllTeams = (subjectId: string) => {
  return queryOptions({
    queryKey: teamsKeyFactory.loadAllTeams(subjectId),
    queryFn: () =>
      apiCall.get<LoadAllTeams[]>("/Teams/all", {
        params: { subjectId },
      }),
  });
};

export const actionsToTeam = (
  subjectId: string,
  method: TeamMethod,
  teamId?: string
) => {
  return mutationOptions({
    mutationKey: teamsKeyFactory.actionsToTeam(method),
    mutationFn: (data?: { name: string; description: string }) =>
      apiCall.post<loadSubjectType[]>(
        `/subjects/${subjectId}/Teams${
          method === "create" ? "" : `/${teamId}`
        }/${method}`,
        data
      ),
  });
};
export const createTeam = (
  subjectId: string,
  data: { name: string; description: string; techStack: string }
) => {
  return apiCall.post(`/Teams/create`, data, {
    params: { subjectId },
  });
};
