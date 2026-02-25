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
      <Box maxWidth="700px" mx="auto">
        <Typography variant="h4" gutterBottom>
          Painel do Professor
        </Typography>

        <Typography variant="body1" color="text.secondary" mb={4}>
          Gerencie suas turmas, aulas, atividades e eventos acadêmicos.
        </Typography>

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