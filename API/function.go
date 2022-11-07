package spritesshare

import (
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	"strconv"
	"strings"
	"time"

	firestore "cloud.google.com/go/firestore"
	firebase "firebase.google.com/go"
	"github.com/GoogleCloudPlatform/functions-framework-go/functions"
	"google.golang.org/api/iterator"
)

type normalMessage struct {
	Message string `json:"message"`
}

func init() {
	functions.HTTP("SpritesShareHTTP", spritesShareHTTP)
}

// ThoughtsHTTP is an HTTP Cloud Function with a request parameter.
func spritesShareHTTP(w http.ResponseWriter, r *http.Request) {
	// Initial setup
	w.Header().Set("Content-Type", "application/json")
	pathParams := strings.Split(r.URL.Path, "/")

	// Firestore connection
	ctx := context.Background()
	conf := &firebase.Config{ProjectID: "sprites-share"}
	app, err := firebase.NewApp(ctx, conf)
	if err != nil {
		w.WriteHeader(http.StatusBadRequest)
		data := normalMessage{Message: fmt.Sprintf("%v", err)}
		json.NewEncoder(w).Encode(data)
		return
	}

	client, err := app.Firestore(ctx)
	if err != nil {
		w.WriteHeader(http.StatusBadRequest)
		data := normalMessage{Message: fmt.Sprintf("%v", err)}
		json.NewEncoder(w).Encode(data)
		return
	}
	defer client.Close()

	// Routing
	if pathParams[1] == "" && len(pathParams) == 2 && r.Method == "GET" {
		var data struct {
			Message string    `json:"message"`
			Routes  [4]string `json:"routes"`
		}
		data.Message = "Sprites Share API - API for sprites sharing"
		data.Routes[0] = "GET /sprites?limit=X&asc=Y - ListAllSprites"
		data.Routes[1] = "POST /sprites - AddSprites"
		data.Routes[2] = "GET /sprites/:id - GetSprites"
		data.Routes[3] = "POST /sprites/:id - RateSprites"
		w.WriteHeader(http.StatusOK)
		json.NewEncoder(w).Encode(data)
		return
	} else if pathParams[1] == "sprites" {
		if len(pathParams) == 2 {
			if r.Method == "GET" {
				queryLimit, err := strconv.Atoi(r.URL.Query().Get("limit"))
				if err != nil || queryLimit == 0 {
					queryLimit = 100
				}

				queryAsc := firestore.Desc
				if r.URL.Query().Get("asc") == "true" {
					queryAsc = firestore.Asc
				}

				var docs []map[string]interface{}
				iter := client.Collection("sprites").OrderBy("dateCreated", queryAsc).Limit(queryLimit).Documents(ctx)
				for {
					doc, err := iter.Next()
					if err == iterator.Done {
						break
					}
					if err != nil {
						data := normalMessage{Message: fmt.Sprintf("%v", err)}
						w.WriteHeader(http.StatusBadRequest)
						json.NewEncoder(w).Encode(data)
						return
					}

					docs = append(docs, doc.Data())
					docs[len(docs)-1]["id"] = doc.Ref.ID
				}

				w.WriteHeader(http.StatusOK)
				json.NewEncoder(w).Encode(docs)
				return
			} else if r.Method == "POST" {
				// Retrieve BODY params
				var d struct {
					Name        string   `json:"name"`
					Description string   `json:"description"`
					DimensionX  uint16   `json:"dimensionX"`
					DimensionY  uint16   `json:"dimensionY"`
					Tags        []string `json:"tags"`
				}
				if err := json.NewDecoder(r.Body).Decode(&d); err != nil {
					data := normalMessage{Message: fmt.Sprintf("%v", err)}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else if d.Name == "" {
					data := normalMessage{Message: "'name' body param is empty"}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else if d.Description == "" {
					data := normalMessage{Message: "'description' body param is empty"}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else if d.DimensionX == 0 {
					data := normalMessage{Message: "'dimensionX' body param is empty"}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else if d.DimensionY == 0 {
					data := normalMessage{Message: "'dimensionY' body param is empty"}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				}

				// Add to Database
				_, _, err := client.Collection("sprites").Add(ctx, map[string]interface{}{
					"dateCreated": time.Now().UTC(),
					"name":        d.Name,
					"description": d.Description,
					"dimensionX":  d.DimensionX,
					"dimensionY":  d.DimensionY,
					"tags":        d.Tags,
				})
				if err != nil {
					data := normalMessage{Message: fmt.Sprintf("%v", err)}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else {
					data := normalMessage{Message: "Sprites created!"}
					w.WriteHeader(http.StatusOK)
					json.NewEncoder(w).Encode(data)
					return
				}
			}
		} else if len(pathParams) == 3 {
			if r.Method == "GET" {
				dsnap, err := client.Collection("sprites").Doc(pathParams[2]).Get(ctx)
				if err != nil {
					data := normalMessage{Message: fmt.Sprintf("%v", err)}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else {
					data := dsnap.Data()
					data["id"] = dsnap.Ref.ID
					w.WriteHeader(http.StatusOK)
					json.NewEncoder(w).Encode(data)
					return
				}
			} else if r.Method == "POST" {
				// Retrieve BODY params
				var d struct {
					Ratings []string `json:"message"`
				}
				if err := json.NewDecoder(r.Body).Decode(&d); err != nil {
					data := normalMessage{Message: fmt.Sprintf("%v", err)}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				}

				// Add to Database
				_, err := client.Collection("sprites").Doc(pathParams[2]).Update(ctx, []firestore.Update{
					{
						Path:  "dateCreated",
						Value: time.Now().UTC(),
					},
				})
				if err != nil {
					data := normalMessage{Message: fmt.Sprintf("%v", err)}
					w.WriteHeader(http.StatusBadRequest)
					json.NewEncoder(w).Encode(data)
					return
				} else {
					data := normalMessage{Message: "Sprites rated!"}
					w.WriteHeader(http.StatusOK)
					json.NewEncoder(w).Encode(data)
					return
				}
			}
		}
	}

	// Default response
	w.WriteHeader(http.StatusBadRequest)
	data := normalMessage{Message: "Unknown route"}
	json.NewEncoder(w).Encode(data)
}
