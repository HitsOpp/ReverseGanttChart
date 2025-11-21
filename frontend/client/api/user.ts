import { apiCall } from "client/utils";
import { queryOptions } from "@tanstack/react-query";
import { type loadSubjectType } from "client/shared";

const profileKeyFactory = {
  loadPofileData: () => ["loadProfile"],
};

export const loadProfileData = () => {
  return queryOptions({
    queryKey: profileKeyFactory.loadPofileData(),
    queryFn: () => apiCall.get<loadSubjectType[]>("/profile"),
  });
};
