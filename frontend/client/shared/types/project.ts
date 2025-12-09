export interface loadProjectType {
  id: string;
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  createdByName: string;
  taskCount: number;
  createdAt: string;
}

type teamProgress = {
  teamId: string;
  teamName: string;
  status: number;
  completedDate: string;
  completedByName: string;
  completedStageCount: number;
  totalStageCount: number;
  progress: number;
};

export interface ProjectTaskType {
  id: string;
  name: string;
  description: string;
  dueDate?: string;
  priority?: number;
  teamProgress: teamProgress[];
}
