import { useQuery } from "@tanstack/react-query";
import { loadProfileData } from "@/api";

export const useProfile = () => {
  return useQuery(loadProfileData());
};
