type Props = {
  selectedRole: "ADMIN" | "PROFESSOR" | null;
  onSelect: (role: "ADMIN" | "PROFESSOR") => void;
};

export default function RoleSelector({ selectedRole, onSelect }: Props) {
  return (
    <div>
      <button onClick={() => onSelect("ADMIN")}>
        Administrador
      </button>

      <button onClick={() => onSelect("PROFESSOR")}>
        Professor
      </button>
    </div>
  );
}
