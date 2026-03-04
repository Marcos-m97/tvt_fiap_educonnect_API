import { Box, Paper } from "@mui/material";
import LoginForm from "../../components/login/LoginForm";
import AuthHeader from "../../components/layout/AuthHeader";

export default function Login() {
  return (
    <Box
      minHeight="100vh"
      display="flex"
      alignItems="center"
      justifyContent="center"
      sx={{
        backgroundColor: "primary.main",
        px: 2,
        py: 6
      }}
    >
      <Paper
        elevation={12}
        sx={{
          width: 420,
          minHeight: 380,
          p: 4,
          borderRadius: 3
        }}
      >
        <AuthHeader
          subtitle="Conectando você à educação"
          showThemeToggle={false}
        />

        <LoginForm />
      </Paper>
    </Box>
  );
}