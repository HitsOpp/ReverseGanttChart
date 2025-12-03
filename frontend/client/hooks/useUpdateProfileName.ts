import { useMutation, useQueryClient } from "@tanstack/react-query";
import { updateProfileName, profileKeyFactory } from "client/api/user";

export const useUpdateProfileName = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: updateProfileName,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: profileKeyFactory.loadProfileData(),
      });
    },
  });
};
