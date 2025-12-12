import { apiCall } from "client/utils";
import { queryOptions } from "@tanstack/react-query";
import { type LoadAllTeams } from "client/shared";

export const teamsKeyFactory = {
  loadAllTeams: (subjectId: string) => [subjectId, "teams"],
  loadMyTeam: (subjectId: string) => [subjectId, "myTeam"],
};
export interface TeamType {
  id: string;
  name: string;
  description: string;
  createdByName: string;
  memberCount: number;
  createdAt: string;
  members: {
    userId: string;
    fullName: string;
    email: string;
    techStack: string;
    joinedAt: string;
  }[];
}

export const loadAllTeams = (subjectId: string) => {
  return queryOptions({
    queryKey: teamsKeyFactory.loadAllTeams(subjectId),
    queryFn: () =>
      apiCall.get<LoadAllTeams[]>("/Teams/all", {
        params: { subjectId },
      }),
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

export const loadMyTeam = (subjectId: string) => {
  return queryOptions({
    queryKey: teamsKeyFactory.loadMyTeam(subjectId),
    queryFn: () =>
      apiCall.get<TeamType>("/Teams/my-team", {
        params: { subjectId },
      }),
  });
};
export const loadTeamInformation = (teamId: string) => {
  return queryOptions({
    queryKey: ["teamInformation", teamId],
    queryFn: () =>
      apiCall.get<LoadAllTeams>(`/Teams/information`, {
        params: { teamId },
      }),
  });
};
