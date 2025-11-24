import { apiCall } from "client/utils";
import { queryOptions } from "@tanstack/react-query";
import { type loadSubjectType } from "client/shared";

type TeamMethod = "create" | "join" | "leave";
const teamsKeyFactory = {
  loadSubject: () => ["loadSubject"],
  actionsToTeam: (method: TeamMethod) => [method, "actionsToTeam"],
};

export const loadTeamsData = () => {
  return queryOptions({
    queryKey: teamsKeyFactory.loadSubject(),
    queryFn: () => apiCall.get<loadSubjectType[]>("/subjects"),
  });
};
export const actionsToTeam = (
  subjectId: string,
  teamId: string,
  method: TeamMethod
) => {
  return queryOptions({
    queryKey: teamsKeyFactory.actionsToTeam(method),
    queryFn: () =>
      apiCall.post<loadSubjectType[]>(
        `/subjects/${subjectId}/Teams/${teamId}/${method}`
      ),
  });
};
