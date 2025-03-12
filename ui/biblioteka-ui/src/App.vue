<script setup>
import {ref, reactive, onMounted, toRaw} from 'vue';
import axios from 'axios';
import config from "@/config";
import Modal from "./components/Modal.vue";
import useVuelidate from '@vuelidate/core';
import { required, maxLength, between } from '@vuelidate/validators';


//console.log(config.API_URL);

  //Filmy
    const title="Moje Filmy";
    const movies = ref([]);
    const newMovies = ref([]);

    //Modal Handling:
    const showModal=ref(false);
    const modalType=ref(0);
    const modalTitle=ref('');
    const movieEditId = ref('');
    
      //Typy okna modalnego
        // 0 -Tryb pasywny
        // 1 - Tryb dodawania
        // 2 - Tryb Edycji

    const openModal = (x,movie) => {
      showModal.value = true;
      if(x==1){
        modalTitle.value="Formularz Dodający";
        modalType.value=1;
      }else if(x==2){
        modalTitle.value="Formularz Edytujący";
        modalType.value=2;
        movieEditId.value=movie.Id; //Id filmu który właśnie edytujemy

        //Placeholdery Dla inputów (placeholdery z wartościami edytowanego filmu):
        formPlaceholders.title=movie.Title;
        formPlaceholders.director=movie.Director;
        formPlaceholders.year=movie.Year;
        formPlaceholders.rate=movie.Rate;

        //Usunięcie domyślnej wartości 0 z pól year i rate, aby widać było placeholder
        formData.year='';
        formData.rate='';
      }

    };

    const closeModal = () => {
      showModal.value = false;
      //Czyszczenie ID edytowanego filmu
      movieEditId.value='';

      //Czyszczenie pól formularza po wyjściu z okna modalnego
      formData.title="";
      formData.director="";
      formData.year=0;
      formData.rate=0;

      //Czyszczenie placeholderów po wyjściu z formularza
      formPlaceholders.title=""
      formPlaceholders.director="";
      formPlaceholders.year="";
      formPlaceholders.rate="";
    };

    //Form handling:
    const formData = reactive({
      title:"",
      director:"",
      rate:0,
      year:0,
    });
    const formPlaceholders = reactive({
      title:"",
      director:"",
      rate:"",
      year:"",
    });

    const rules = {
      title:{required, maxLength: maxLength(200)},
      director:{},
      rate:{},
      year:{between: between(1900,2200)},
    }

    const v$ = useVuelidate(rules,formData);
    //Wyświetlanie zasad formularza od razu:
    v$.value.$touch();

    //console.log(v$);
    

    //Przełączenie typu formularza:
    const submitForm = (x) =>{
      if(x==='1'){
        submitFormAdd();
      }else if(x==='2'){
        submitFormUpdate();
      }
    }

    //Metoda dla formularza dodającego
    const submitFormAdd = async () =>{
      const result = await v$.value.$validate();
      console.log(result);
      if(result){
        //console.log("Formularz poprawny, dane:", formData.value);
        const postMovie ={
        id: 0, 
        director: formData.director, 
        title: formData.title,
        year: formData.year,
        rate: formData.rate
      }
        await addMovie(postMovie);
        await getMovies();
        closeModal();
        
      }else{
        alert("Nie udało się dodać filmu"+v$.value.$errors);
        
      }
    }

    //Metoda dla formularza edytującego
    const submitFormUpdate = async () =>{
      const result = await v$.value.$validate();
      
      if(result){
          //Jeśli użytkownik pominie pole to zostanie pobrana własciwość z placeholdera + nowy obiekt do Puta
          const putObject = toRaw(formData);
          for(const key in putObject){
            if(putObject[key]==='' || putObject[key]===null || putObject[key]===undefined){
              putObject[key] = formPlaceholders[key];
            }
          }
          // console.log(movieEditId.value);
          // console.log(putObject);

          await updateMovie(movieEditId.value,
          {
            "id":movieEditId.value ,
            "director": putObject.director,
            "title": putObject.title,
            "year": putObject.year,
            "rate": putObject.rate
          }
          );
          await getMovies();
          closeModal();
        }else{
          alert("Nie udało się edytować filmu");
        }
      
    }


    //HTTP Requesty:
    //GET 
    const getMovies = async () =>{
      try {
          const response = await axios.get(config.API_URL+"/api/Movies");
          movies.value = response.data;
        } catch (err) {
            //console.log(err);
            errors.getMoviesError=true;
            errors.getMoviesErrorMsg=err.message;
            console.log(errors.getMoviesErrorMsg);
            //Błędy pokazywane mają być u użytkownika
        }
    }
    //GET z API rekrutacyjnego
    const getNewMovies = async () =>{
      try {
          const response = await axios.get(config.exAPI_URL_GET);
          newMovies.value = response.data;
          return true;
        } catch (err) {
            //console.log(err);
            errors.externalApiError=true;
            errors.externalApiErrorMsg=err.message;
            return false;
        }
    }
    //GET {id}
    // const getMoviesId = async (x) =>{
    //   try {
    //       const response = await axios.get(config.API_URL+"/api/Movies/"+x);
    //       return response.data;
    //     } catch (err) {
    //         console.log(err);
    //         //Błędy pokazywane mają być u użytkownika
    //     }
    // }
    //DELETE
    const delMovie = async (id) =>{
      const confirmDelete = confirm("Czy na pewno chcesz usunąć ten film?");
      if (!confirmDelete) return;
      try{
        await axios.delete(config.API_URL+"/api/Movies/"+id);
        movies.value = movies.value.filter(movie => movie.id !== id);
        getMovies();
      }catch(err){
        errors.delMovieError=true;
        errors.delMovieErrorMsg=err.message;
        //console.log(errors.delMovieErrorMsg);
      }
      
    }
    //POST
    const addMovie = async (x) =>{
      
      try{
        await axios.post(config.API_URL+"/api/Movies",x)
      }catch(err){
        errors.postMovieError=true;
        errors.postMovieErrorMsg=err.message;
      }
    }
    //PUT
    const updateMovie = async (id,x) =>{
      try{
        const response = await axios.put(config.API_URL+"/api/Movies/"+id,x);
        console.log('Film zaktualizowany:', response.data);
      }catch(err){
        //console.error(err);
        errors.postMovieError=true;
        errors.postMovieErrorMsg=err.message;
      }
    }

    //Osobna metoda do filmów z zewnętrznego API
    const newMoviesMethod = async ()=>{
      const state = await getNewMovies();
      if(state){
        //Udało się pobrać z API
        //tablica newMovies posiada filmy z nowego Api
        console.log(newMovies.value);
        for(const movie of newMovies.value){
          //console.log(movie.id);
          //Konflikt IDków:
          movie.id=0;
          //console.log(movie)
          addMovie(movie);
        }
        //console.log(movies.value)
        await getMovies();

      }else{
        //Nie udało się pobrać z API
      }
    }
//Obiekt do errorów API:
const errors = reactive({
      getMovieError: false,
      getMovieErrorMsg:'',

      delMovieError: false,
      delMovieErrorMsg:'',
      
      postMovieError:false,
      postMovieErrorMsg:"",

      putMovieError:false,
      putMovieErrorMsg:"",

      externalApiError:false,
      externalApiErrorMsg:"",
    });
    

const isAPIon = async ()=>{
  try {
    const response = await axios.get(config.API_URL+'/api/Movies');
    
    if (response.status === 200) {
      await getMovies();
    }
    } catch (error) {
      errors.getMovieError=true;
      errors.getMoviesErrorMsg=error.message;
      movies.value=[];
      await getMovies();
  }
}
    

    onMounted(()=>{
      getMovies();
      setInterval(isAPIon, 10000);
    });
</script>
<template>
  <div class="page-header text-center"><h1>{{ title }}</h1></div>
  <div class="d-flex justify-content-center btn-group">
    <button class="btn btn-primary" @click="openModal(1)">Dodaj</button>
    <button class="btn btn-success" @click="newMoviesMethod">Pobierz filmy</button>
  </div>
    <table class="table">
        <thead>
          <tr class="text-center">
            <th scope="col">Tytuł</th>
            <th scope="col">Reżyser</th>
            <th scope="col">Rok</th>
            <th scope="col">Ocena</th>
            <th scope="col">Opcje</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="movie in movies" :key="movie.id" class="text-center">
              <td>{{ movie.Title }}</td>
              <td>{{ movie.Director }}</td>
              <td>{{ movie.Year }}</td>
              <td>{{ movie.Rate+"/10" }}</td>
              <td><div class="btn-group">
                <button class="btn btn-outline-warning" @click="openModal(2,movie)">Edytuj</button>
                <button class="btn btn-outline-danger" @click="delMovie(movie.Id)">Usuń</button>
              </div>
              </td> 
          </tr>
        </tbody>
    </table>
    <!--Errory API-->
    <div v-if="errors.getMovieError" class="alert alert-danger text-center">Nie można wyświetlić filmów: {{errors.getMoviesErrorMsg }}
      <button @click="errors.getMovieError=false" class="btn btn-outline-danger">OK</button>
    </div>
    <div v-if="errors.delMovieError" class="alert alert-danger text-center">Nie udało się usunąć filmu: {{ errors.delMovieErrorMsg }}
      <button @click="errors.delMovieError=false" class="btn btn-outline-danger">OK</button>
    </div>
    <div v-if="errors.postMovieError" class="alert alert-warning text-center">Nie udało się dodać filmu: {{ errors.postMovieErrorMsg }}
      <button @click="errors.postMovieError=false" class="btn btn-outline-warning">OK</button>
    </div>
    <div v-if="errors.putMovieError" class="alert alert-warning text-center">Nie udało się edytować filmu: {{ errors.putMovieErrorMsg }}
      <button @click="errors.putMovieError=false" class="btn btn-outline-warning">OK</button>
    </div>
    <div v-if="errors.externalApiError" class="alert alert-warning text-center">Nie udało się pobrać filmów: {{ errors.externalApiErrorMsg }}
      <button @click="errors.externalApiError=false" class="btn btn-outline-warning">OK</button>
    </div>
    <!-- Modal -->
    <Modal :show="showModal" @close="closeModal">
      <template #header>
        <h3>{{modalTitle}} {{ movieEditId }}</h3>
      </template>
      
      <form @submit.prevent="submitForm()">
        <div class="mb-3">
          <label for="AddTitle" class="form-label">Podaj Tytuł filmu</label>
          <input v-model="formData.title" :placeholder="formPlaceholders.title" id="AddTitle" type="text" class="form-control">
          <span v-if="v$.title.$error">Wartość jest wymagana</span>
        </div>
        <div class="mb-3">
          <label for="AddDirector" class="form-label">Podaj Reżysera filmu</label>
          <input v-model="formData.director" :placeholder="formPlaceholders.director" id="AddDirector" type="text" class="form-control">
        </div>
        <div class="mb-3">
          <label for="AddYear" class="form-label">Podaj rok</label>
          <input v-model="formData.year" :placeholder="formPlaceholders.year" id="AddYear" type="number" class="form-control">
          <span v-if="v$.year.$error" class="form-text">Wartość musi mieścić się w zakresie od 1900 do 2200</span>
        </div>
        <div class="mb-3">
          <label for="AddRate" class="form-label">Podaj ocenę</label>
          <input v-model="formData.rate" :placeholder="formPlaceholders.rate" id="AddRate" type="number" class="form-control">
        </div>

        <div class="btn-group">
          <button class="btn btn-danger" @click="closeModal">Anuluj</button>
          <button class="btn btn-success" :class="{ disabled: v$.$invalid }" v-if="modalType==1" @click="submitForm('1')">Zapisz</button>
          <button class="btn btn-success " :class="{ disabled: v$.$invalid }" v-if="modalType==2" @click="submitForm('2')">Edytuj</button>
        </div>
      </form>
      <template #footer>
        
      </template>
    </Modal>
</template>

<style scoped>
#app{
  position:relative;
}

</style>


