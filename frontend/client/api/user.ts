import { apiCall } from "client/utils";
import { queryOptions } from "@tanstack/react-query";
import type { loadProfileDataResponse } from "@/shared";

export const profileKeyFactory = {
  loadProfileData: () => ["loadProfile"],
};

export const loadProfileData = () => {
  return queryOptions({
    queryKey: profileKeyFactory.loadProfileData(),
    queryFn: () => apiCall.get<loadProfileDataResponse>("/User/profile"),
  });
};

export const updateProfileName = (fullName: string) => {
  return apiCall.put("/Auth/profile", { fullName });
};