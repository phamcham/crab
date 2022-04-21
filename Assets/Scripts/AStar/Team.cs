public class Team{
    public int id;
    public Team(int id){
        this.id = id;
    }

    public static Team DefaultPlayer => new Team(1);
    public static Team DefaultEnemy => new Team(2);

    public bool IsEnemy(Team other){
        return id != other.id;
    }
}