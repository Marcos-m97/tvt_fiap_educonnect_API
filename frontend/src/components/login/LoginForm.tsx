import { Button, TextField, Stack } from "@mui/material";

type Props = {
  role: "ADMIN" | "PROFESSOR" | "ALUNO";
};

export default function LoginForm({ role }: Props) {
  return (
    <Stack spacing={2}>
      <TextField label="Email" type="email" fullWidth />
      <TextField label="Senha" type="password" fullWidth />

      <Button variant="contained" fullWidth>
        Entrar como {role}
      </Button>
    </Stack>
  );
}
