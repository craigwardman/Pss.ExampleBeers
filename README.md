# Pss.ExampleBeers
Coding exercise - Beers, Bars, Breweries API

Produce a .NET Core Web Api which stores, updates and retrieves different types of beer, their associated breweries and bars that serve the beers. Ensure the key logic is covered by some unit tests.

## Beer model


| Property                  | Type    |
| ------------------------- | ------- |
| Name                      | string  |
| PercentageAlcoholByVolume | decimal |

## Brewery


| Property                  | Type    |
| ------------------------- | ------- |
| Name                      | string  |

## Bar

| Property                  | Type    |
| ------------------------- | ------- |
| Name                      | string  |
| Address                   | string  |

Ensure this data model is represented in such a way that:

- Breweries can have many beers
- Bars can serve many types of beers

Create a .NET Core Web Api with the following endpoints available:

## Beers

```
- POST /beer - Insert a single beer
- PUT /beer/{id} - Update a beer by Id
- GET /beer?gtAlcoholByVolume=5.0&ltAlcoholByVolume=8.0 - Get all beers with optional filtering query parameters for alcohol content (gtAlcoholByVolume = greater than, ltAlcoholByVolume = less than)
- GET /beer/{id} - Get beer by Id
```

## Breweries

```
- POST /brewery - Insert a single brewery
- PUT /brewery/{id} - Update a brewery by Id
- GET /brewery - Get all breweries
- GET /brewery/{id} - Get brewery by Id
```

## Brewery Beers

```
- POST /brewery/beer - Insert a single brewery beer link
- GET /brewery/{breweryId}/beer - Get a single brewery by Id with associated beers
- GET /brewery/beer - Get all breweries with associated beers
```

## Bars

```
- POST /bar - Insert a single bar
- PUT /bar/{id} - Update a bar by Id
- GET /bar - Get all bars
- GET /bar/{id} - Get bar by Id
```

## Bar Beers

```
- POST /bar/beer - Insert a single bar beer link
- GET /bar/{barId}/beer - Get a single bar with associated beers
- GET /bar/beer - Get all bars with associated beers
