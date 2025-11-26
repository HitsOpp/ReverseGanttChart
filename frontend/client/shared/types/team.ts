export interface LoadAllTeams {
  id: string;
  name: string;
  memberCount: number;
  description: string;
  createdAt: string;
  createdByName: string;
  members: Array<{
    userId: string;
    fullName: string;
    email: string;
    techStack: string;
    joinedAt: string;
  }>;
}
