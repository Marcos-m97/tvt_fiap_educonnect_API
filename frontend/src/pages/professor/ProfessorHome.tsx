import {
  Typography,
  Box,
  Card,
  CardContent,
  CardActions,
  Button,
  Divider
} from "@mui/material";
import AppLayout from "../../components/layout/AppLayout";
import SchoolIcon from "@mui/icons-material/School";
import EventIcon from "@mui/icons-material/Event";
import { useNavigate } from "react-router-dom";

export default function ProfessorHome() {
  const navigate = useNavigate();

  return (
    <AppLayout>
      <Box maxWidth="800px" mx="auto">

        {/* HEADER CENTRALIZADO */}
        <Box textAlign="center" mb={6}>
          <Typography
            variant="h3"
            sx={{
              fontWeight: 800,
              letterSpacing: "0.08em",
              mb: 2
            }}
          >
            Painel do{" "}
            <Box component="span" sx={{ color: "primary.main" }}>
              Professor
            </Box>
          </Typography>

          <Typography
            variant="h6"
            color="text.secondary"
            sx={{
              fontWeight: 400,
              maxWidth: "600px",
              mx: "auto"
            }}
          >
            Gerencie suas turmas, aulas, atividades e eventos acadêmicos.
          </Typography>
        </Box>

        {/* Card Acadêmico */}
        <Card
          sx={{
            mb: 4,
            transition: "0.2s",
            "&:hover": {
              boxShadow: 6,
              transform: "translateY(-4px)"
            }
          }}
        >
          <CardContent>
            <Box display="flex" alignItems="center" gap={2}>
              <SchoolIcon fontSize="large" />
              <Typography variant="h6">
                Acadêmico
              </Typography>
            </Box>

            <Typography variant="body2" mt={2}>
              Acesse suas turmas, gerencie aulas, atividades, alunos e notas.
            </Typography>
          </CardContent>

          <CardActions>
            <Button
              size="small"
              onClick={() => navigate("/professor/academico")}
            >
              Acessar
            </Button>
          </CardActions>
        </Card>

        <Divider sx={{ mb: 4 }} />

        {/* Card Eventos */}
        <Card
          sx={{
            transition: "0.2s",
            "&:hover": {
              boxShadow: 6,
              transform: "translateY(-4px)"
            }
          }}
        >
          <CardContent>
            <Box display="flex" alignItems="center" gap={2}>
              <EventIcon fontSize="large" />
              <Typography variant="h6">
                Eventos
              </Typography>
            </Box>

            <Typography variant="body2" mt={2}>
              Criar e gerenciar eventos acadêmicos e comunicados.
            </Typography>
          </CardContent>

          <CardActions>
            <Button
              size="small"
              onClick={() => navigate("/professor/eventos")}
            >
              Acessar
            </Button>
          </CardActions>
        </Card>

      </Box>
    </AppLayout>
  );
}