import {
  Typography,
  Box,
  Card,
  CardContent,
  CardActions,
  Button
} from "@mui/material";
import AppLayout from "../../../components/layout/AppLayout";
import SchoolIcon from "@mui/icons-material/School";
import ClassIcon from "@mui/icons-material/Class";
import GroupsIcon from "@mui/icons-material/Groups";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { useNavigate } from "react-router-dom";

export default function AdminAcademicoHome() {
  const navigate = useNavigate();

  const cards = [
    {
      title: "Cursos",
      description: "Gerencie os cursos da instituição.",
      icon: <SchoolIcon fontSize="large" />,
      route: "/admin/academico/cursos"
    },
    {
      title: "Disciplinas",
      description: "Gerencie as disciplinas vinculadas aos cursos.",
      icon: <ClassIcon fontSize="large" />,
      route: "/admin/academico/disciplinas"
    },
    {
      title: "Turmas",
      description: "Gerencie as turmas por curso e semestre.",
      icon: <GroupsIcon fontSize="large" />,
      route: "/admin/academico/turmas"
    }
  ];

  return (
    <AppLayout>

      {/* HEADER */}
      <Box mb={4} textAlign="center">
        <Typography variant="h4" gutterBottom>
          Estrutura Acadêmica
        </Typography>

        <Typography variant="body1" color="text.secondary">
          Gerencie cursos, disciplinas e turmas da instituição.
        </Typography>
      </Box>

      {/* BOTÃO VOLTAR */}
      <Box mb={4}>
        <Button
          variant="outlined"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate("/admin")}
          sx={{ textTransform: "none" }}
        >
          Voltar
        </Button>
      </Box>

      {/* CARDS */}
      <Box
        display="grid"
        gridTemplateColumns={{
          xs: "1fr",
          md: "1fr 1fr"
        }}
        gap={3}
      >
        {cards.map((card, index) => (
          <Card
            key={index}
            sx={{
              height: "100%",
              transition: "0.2s",
              "&:hover": {
                boxShadow: 6,
                transform: "translateY(-4px)"
              }
            }}
          >
            <CardContent>
              <Box display="flex" alignItems="center" gap={2}>
                {card.icon}
                <Typography variant="h6">
                  {card.title}
                </Typography>
              </Box>

              <Typography variant="body2" mt={2}>
                {card.description}
              </Typography>
            </CardContent>

            <CardActions>
              <Button
                size="small"
                onClick={() => navigate(card.route)}
              >
                Acessar
              </Button>
            </CardActions>
          </Card>
        ))}
      </Box>

    </AppLayout>
  );
}