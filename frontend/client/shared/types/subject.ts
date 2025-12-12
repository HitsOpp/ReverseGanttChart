export interface loadSubjectType {
  name: string;
  description: string;
  id: string;
  assistantId: number;
  creatorName: string;
  headerColor?: string;
  color?: string;
  currentUserRole: "teacher" | "student" | "assistant";
}
