export interface loadSubjectType {
  name: string;
  description: string;
  id: string;
  assistantId: number;
  creatorName: string;
  headerColor:
    | "blue"
    | "green"
    | "red"
    | "purple"
    | "orange"
    | "indigo"
    | "pink";
  currentUserRole: "teacher" | "student" | "assistant";
}
